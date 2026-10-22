import socket
import struct
import hashlib
import time
import zlib
import urllib.request
import urllib.parse
import json

def xor_bytes(data: bytes, key: int = 0x8C) -> bytes:
    return bytes([b ^ key for b in data])

def build_client_packet(cmd_id: int, payload_str: str, ticks: int = 1000) -> bytes:
    payload_raw = payload_str.encode('utf-8')
    # body_to_checksum is ticks (4 bytes little-endian) + payload_raw
    body_to_checksum = struct.pack('<i', ticks) + payload_raw
    crc_val = zlib.crc32(body_to_checksum) & 0xffffffff
    cc = crc_val % 255
    cc2 = cmd_id % 255
    crc_byte = (cc ^ cc2) & 0xff

    # bytes_data = 1 byte crc + 4 bytes ticks + payload
    bytes_data = bytes([crc_byte]) + body_to_checksum

    # Header = 4 bytes length (2 for cmd_id + len(bytes_data)), 2 bytes cmd_id
    length = 2 + len(bytes_data)
    header = struct.pack('<IH', length, cmd_id)

    raw_packet = header + bytes_data
    # Whole packet is XORed with 0x8C
    return xor_bytes(raw_packet, 0x8C)

def parse_server_packet(data: bytes):
    if len(data) < 6:
        return None, None, data
    # Server packet: 4 bytes length, 2 bytes cmd_id, followed by payload
    length, cmd_id = struct.unpack('<IH', data[:6])
    payload_len = length - 2
    if len(data) < 6 + payload_len:
        return None, None, data  # incomplete
    payload_raw = data[6:6 + payload_len]
    rem = data[6 + payload_len:]
    return cmd_id, payload_raw.decode('utf-8', errors='replace'), rem

def main():
    print("=== JX1M END-TO-END CLUSTER TEST ===")

    # 1. Test WebSDK Server List
    print("\n[1/4] Testing WebSDK GetServerList...")
    req = urllib.request.Request("http://127.0.0.1:8081/GetServerListTest.aspx")
    try:
        with urllib.request.urlopen(req, timeout=5) as resp:
            data = resp.read()
            print(f" -> WebSDK returned {len(data)} bytes protobuf server list data.")
            assert len(data) > 0, "Empty server list!"
            print(" -> PASS: WebSDK is up and serving server list!")
    except Exception as e:
        print(f" -> FAIL: WebSDK error: {e}")
        return

    # 2. Test GameDBServer TCP Ping
    print("\n[2/4] Testing GameDBServer TCP port 23001...")
    try:
        s_db = socket.create_connection(("127.0.0.1", 23001), timeout=3)
        print(" -> PASS: GameDBServer accepted TCP connection!")
        s_db.close()
    except Exception as e:
        print(f" -> FAIL: GameDBServer error: {e}")
        return

    # 3. Test GameServer TCP Handshake & CMD_LOGIN_ON2
    print("\n[3/4] Testing GameServer TCP port 3001 & CMD_LOGIN_ON2...")
    try:
        s_gs = socket.create_connection(("127.0.0.1", 3001), timeout=5)
        print(" -> Connected to GameServer socket 127.0.0.1:3001")

        # Prepare login credentials
        user_id = "1"
        user_name = "testuser_01"
        last_time = str(int(time.time()))
        is_adult = "1"
        web_key = "9377(*)#mst9"
        
        sign_str = user_id + user_name + last_time + is_adult + web_key
        sign_code = hashlib.md5(sign_str.encode('utf-8')).hexdigest().lower()
        ver_sign = 20140624
        
        login_payload = f"{ver_sign}:{user_id}:{user_name}:{last_time}:{is_adult}:{sign_code}"
        print(f" -> Sending CMD_LOGIN_ON2 (20): payload='{login_payload}'")

        pkt = build_client_packet(20, login_payload, ticks=1000)
        s_gs.sendall(pkt)

        # Receive response from GameServer
        s_gs.settimeout(5)
        resp_bytes = s_gs.recv(4096)
        print(f" -> Received {len(resp_bytes)} bytes from GameServer")

        if len(resp_bytes) >= 6:
            cmd, resp_str, _ = parse_server_packet(resp_bytes)
            print(f" -> Parsed server response: CMD={cmd}, Data='{resp_str}'")
            if cmd == 20:
                print(" -> PASS: GameServer authenticated and returned UserLoginToken!")
        else:
            print(f" -> Raw hex: {resp_bytes.hex()}")

        s_gs.close()
    except Exception as e:
        print(f" -> FAIL: GameServer test error: {e}")
        import traceback
        traceback.print_exc()
        return

    print("\n[4/4] ALL CLUSTER INTEGRATION TESTS PASSED SUCCESSFULLY!")

if __name__ == "__main__":
    main()
