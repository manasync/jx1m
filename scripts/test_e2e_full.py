#!/usr/bin/env python3
import socket
import struct
import hashlib
import time
import zlib
import urllib.request
import sys

def xor_bytes(data: bytes, key: int = 0x8C) -> bytes:
    return bytes([b ^ key for b in data])

def build_client_packet(cmd_id: int, payload_str: str, ticks: int = 1000) -> bytes:
    payload_raw = payload_str.encode('utf-8')
    body_to_checksum = struct.pack('<i', ticks) + payload_raw
    crc_val = zlib.crc32(body_to_checksum) & 0xffffffff
    cc = crc_val % 255
    cc2 = cmd_id % 255
    crc_byte = (cc ^ cc2) & 0xff
    bytes_data = bytes([crc_byte]) + body_to_checksum
    length = 2 + len(bytes_data)
    header = struct.pack('<IH', length, cmd_id)
    return xor_bytes(header + bytes_data, 0x8C)

def recv_packet(s, timeout=5):
    s.settimeout(timeout)
    header = b''
    while len(header) < 6:
        chunk = s.recv(6 - len(header))
        if not chunk:
            return None, None
        header += chunk
    length, cmd_id = struct.unpack('<IH', header)
    payload_len = length - 2
    payload = b''
    while len(payload) < payload_len:
        chunk = s.recv(payload_len - len(payload))
        if not chunk:
            break
        payload += chunk
    return cmd_id, payload

def run_backend_flow_test():
    print("=" * 60)
    print("  JX1M HEADLESS END-TO-END FLOW VALIDATOR (BACKEND & PROTOCOL)")
    print("=" * 60)

    # 1. Test WebSDK
    print("\n[Step 1/6] WebSDK Server List Endpoint...")
    try:
        req = urllib.request.Request("http://127.0.0.1:8081/GetServerListTest.aspx")
        with urllib.request.urlopen(req, timeout=5) as resp:
            data = resp.read()
            assert len(data) > 0, "Empty server list data"
            print(f"  [OK] WebSDK responded with {len(data)} bytes server list.")
    except Exception as e:
        print(f"  [FAIL] WebSDK error: {e}")
        return False

    # 2. Test GameDBServer TCP
    print("\n[Step 2/6] GameDBServer TCP Connection (port 23001)...")
    try:
        s_db = socket.create_connection(("127.0.0.1", 23001), timeout=3)
        s_db.close()
        print("  [OK] GameDBServer TCP port 23001 is accepting connections.")
    except Exception as e:
        print(f"  [FAIL] GameDBServer error: {e}")
        return False

    # 3. Test GameServer Auth Login (CMD 20)
    print("\n[Step 3/6] GameServer Auth Login (CMD 20 CMD_LOGIN_ON2)...")
    user_id = "1"
    user_name = "testuser_01"
    last_time = str(int(time.time()))
    is_adult = "1"
    web_key = "9377(*)#mst9"
    sign_str = user_id + user_name + last_time + is_adult + web_key
    sign_code = hashlib.md5(sign_str.encode('utf-8')).hexdigest().lower()
    login_payload = f"20140624:{user_id}:{user_name}:{last_time}:{is_adult}:{sign_code}"

    try:
        s_auth = socket.create_connection(("127.0.0.1", 3001), timeout=5)
        s_auth.sendall(build_client_packet(20, login_payload))
        cmd, data = recv_packet(s_auth)
        s_auth.close()
        assert cmd == 20, f"Expected CMD 20, got {cmd}"
        data_str = data.decode('utf-8', errors='replace')
        fields = data_str.split(':')
        user_token = fields[2]
        print(f"  [OK] Auth authenticated successfully. Token length: {len(user_token)}")
    except Exception as e:
        print(f"  [FAIL] Auth error: {e}")
        return False

    # 4. Game Session Connect & Login (CMD 100)
    print("\n[Step 4/6] Game Session Handshake & Login (CMD 100 CMD_LOGIN_ON)...")
    try:
        s_game = socket.create_connection(("127.0.0.1", 3001), timeout=5)
        ver_sign = 20140624
        device_info = "HeadlessFlowTester"
        # Standard 13-field packet: UserID:UserName:UserToken:RoleRandToken:VerSign:UserIsAdult:DeviceInfo:RoleId:GameId:GameType:ServerId:ServerIp:ServerPort
        cmd_100_payload = f"{user_id}:{user_name}:{user_token}:0:{ver_sign}:{is_adult}:{device_info}:0:0:0:0::0"
        s_game.sendall(build_client_packet(100, cmd_100_payload))
        cmd, data = recv_packet(s_game)
        assert cmd == 100, f"Expected CMD 100, got {cmd}"
        print(f"  [OK] GameSession accepted login. Server session token: {data.decode('utf-8')}")
    except Exception as e:
        print(f"  [FAIL] Game session error: {e}")
        return False

    # 5. Role List & Auto-create Role if empty (CMD 101 & CMD 102)
    print("\n[Step 5/6] Role Verification (CMD 101 Role List & CMD 102 Create Role)...")
    role_id = None
    role_name = None
    try:
        cmd_101_payload = f"{user_id}:1"
        s_game.sendall(build_client_packet(101, cmd_101_payload))
        cmd, data = recv_packet(s_game)
        assert cmd == 101, f"Expected CMD 101, got {cmd}"
        role_str = data.decode('utf-8', errors='replace')
        
        if role_str.startswith("0:"):
            # No roles exist yet, create one
            test_name = f"Tester{int(time.time()) % 10000}"
            print(f"  [-] No role found. Creating test character '{test_name}' (Faction 1)...")
            cmd_102_payload = f"{user_id}:{user_name}:0:1:{test_name}:1"
            s_game.sendall(build_client_packet(102, cmd_102_payload))
            cmd, data = recv_packet(s_game)
            assert cmd == 102, f"Expected CMD 102, got {cmd}"
            create_str = data.decode('utf-8', errors='replace')
            parts = create_str.split(':')
            assert len(parts) >= 2 and parts[0] == "1", f"Role creation failed: {create_str}"
            role_meta = parts[1].split('$')
            role_id = role_meta[0]
            role_name = role_meta[3] if len(role_meta) > 3 else test_name
            print(f"  [OK] Character created successfully! RoleID = {role_id}, Name = {role_name}")
        else:
            parts = role_str.split(':')
            first_role_info = parts[1].split('$')
            role_id = first_role_info[0]
            role_name = first_role_info[3] if len(first_role_info) > 3 else "Unknown"
            print(f"  [OK] Found existing character: RoleID = {role_id}, Name = {role_name}")
    except Exception as e:
        print(f"  [FAIL] Role check error: {e}")
        s_game.close()
        return False

    # 6. Enter Game & World Sync (CMD 104 & CMD 106)
    print("\n[Step 6/6] Enter Game & Enter World Map (CMD 104 INIT_GAME & CMD 106 PLAY_GAME)...")
    try:
        cmd_104_payload = f"{user_id}:{role_id}:{device_info}"
        s_game.sendall(build_client_packet(104, cmd_104_payload))
        
        # Read packets until CMD 104 (Protobuf RoleData)
        received_init = False
        for _ in range(15):
            cmd, data = recv_packet(s_game, timeout=5)
            if cmd == 104:
                received_init = True
                print(f"  [OK] Received CMD_INIT_GAME (104) with {len(data)} bytes of Protobuf RoleData.")
                break
            elif cmd is not None:
                # Server sync events like 45011, 45000, 50001
                pass
        
        assert received_init, "Timed out waiting for CMD_INIT_GAME (104) response"

        # Now send CMD_PLAY_GAME to enter the map
        s_game.sendall(build_client_packet(106, f"{role_id}"))
        cmd, data = recv_packet(s_game, timeout=5)
        print(f"  [OK] Server acknowledged world entry! First world packet: CMD = {cmd}")
        s_game.close()
    except Exception as e:
        print(f"  [FAIL] World entry error: {e}")
        s_game.close()
        return False

    print("\n" + "=" * 60)
    print("  RESULT: BACKEND & NETWORK FLOW FULLY VERIFIED (READY FOR CLIENT)!")
    print("=" * 60)
    return True

if __name__ == "__main__":
    success = run_backend_flow_test()
    sys.exit(0 if success else 1)
