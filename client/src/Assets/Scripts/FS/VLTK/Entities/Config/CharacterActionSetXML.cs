using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FS.VLTK.Entities.Config
{
    /// <summary>
    /// Đối tượng quản lý ActionSet
    /// </summary>
    public class CharacterActionSetXML
    {
        /// <summary>
        /// Thành phần
        /// </summary>
        public class Component
        {
            /// <summary>
            /// Mã của thành phần
            /// </summary>
            public int Code { get; set; }

            /// <summary>
            /// ID thành phần
            /// </summary>
            public string ID { get; set; }

            /// <summary>
            /// Đường dẫn file Bundle chứa thành phần
            /// </summary>
            public string BundleDir { get; set; }
        }

        #region Male
        /// <summary>
        /// Quần áo nhân vật nam
        /// </summary>
        public Dictionary<string, Component> MaleArmor { get; set; } = new Dictionary<string, Component>();
        /// <summary>
        /// Quần áo nhân vật nam theo mã thành phần
        /// </summary>
        public Dictionary<int, Component> MaleArmorByCode { get; set; } = new Dictionary<int, Component>();

        /// <summary>
        /// Đầu nhân vật nam
        /// </summary>
        public Dictionary<string, Component> MaleHead { get; set; } = new Dictionary<string, Component>();
        /// <summary>
        /// Đầu nhân nhân vật nam theo mã thành phần
        /// </summary>
        public Dictionary<int, Component> MaleHeadByCode { get; set; } = new Dictionary<int, Component>();

        /// <summary>
        /// Phi phong nhân vật nam
        /// </summary>
        public Dictionary<string, Component> MaleMantle { get; set; } = new Dictionary<string, Component>();
        /// <summary>
        /// Phi phong nhân vật nam theo mã thành phần
        /// </summary>
        public Dictionary<int, Component> MaleMantleByCode { get; set; } = new Dictionary<int, Component>();

        /// <summary>
        /// Vũ khí nhân vật nam
        /// </summary>
        public Dictionary<string, Component> MaleWeapon { get; set; } = new Dictionary<string, Component>();
        /// <summary>
        /// Vũ khí nhân vật nam theo mã thành phần
        /// </summary>
        public Dictionary<int, Component> MaleWeaponByCode { get; set; } = new Dictionary<int, Component>();
        #endregion

        #region Female
        /// <summary>
        /// Quần áo nhân vật nữ
        /// </summary>
        public Dictionary<string, Component> FemaleArmor { get; set; } = new Dictionary<string, Component>();
        /// <summary>
        /// Quần áo nhân vật nữ theo mã thành phần
        /// </summary>
        public Dictionary<int, Component> FemaleArmorByCode { get; set; } = new Dictionary<int, Component>();

        /// <summary>
        /// Đầu nhân vật nữ
        /// </summary>
        public Dictionary<string, Component> FemaleHead { get; set; } = new Dictionary<string, Component>();
        /// <summary>
        /// Đầu nhân nhân vật nữ theo mã thành phần
        /// </summary>
        public Dictionary<int, Component> FemaleHeadByCode { get; set; } = new Dictionary<int, Component>();

        /// <summary>
        /// Phi phong nhân vật nữ
        /// </summary>
        public Dictionary<string, Component> FemaleMantle { get; set; } = new Dictionary<string, Component>();
        /// <summary>
        /// Phi phong nhân vật nữ theo mã thành phần
        /// </summary>
        public Dictionary<int, Component> FemaleMantleByCode { get; set; } = new Dictionary<int, Component>();

        /// <summary>
        /// Vũ khí nhân vật nữ
        /// </summary>
        public Dictionary<string, Component> FemaleWeapon { get; set; } = new Dictionary<string, Component>();
        /// <summary>
        /// Vũ khí nhân vật nữ theo mã thành phần
        /// </summary>
        public Dictionary<int, Component> FemaleWeaponByCode { get; set; } = new Dictionary<int, Component>();
        #endregion

        #region Rider
        /// <summary>
        /// Ngựa
        /// </summary>
        public Dictionary<string, Component> Rider { get; set; } = new Dictionary<string, Component>();
        /// <summary>
        /// Ngựa theo mã thành phần
        /// </summary>
        public Dictionary<int, Component> RiderByCode { get; set; } = new Dictionary<int, Component>();
        #endregion

        /// <summary>
        /// Chuyển đối tượng từ XML Node
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <returns></returns>
        public static CharacterActionSetXML Parse(XElement xmlNode)
        {
            CharacterActionSetXML actionSet = new CharacterActionSetXML();
            if (xmlNode == null) return actionSet;

            System.Action<XElement, string, System.Collections.Generic.Dictionary<string, Component>, System.Collections.Generic.Dictionary<int, Component>> parsePart = (parent, partName, dictById, dictByCode) =>
            {
                if (parent == null) return;
                XElement partNode = parent.Element(partName);
                if (partNode == null) return;
                foreach (XElement data in partNode.Elements("Set"))
                {
                    int code = int.Parse(data.Attribute("Code").Value);
                    string id = data.Attribute("ID").Value;
                    string bundleDir = data.Attribute("BundleDir").Value;
                    Component comp = new Component()
                    {
                        Code = code,
                        ID = id,
                        BundleDir = bundleDir,
                    };
                    dictById[id] = comp;
                    dictByCode[code] = comp;
                }
            };

            #region Male
            XElement maleNode = xmlNode.Element("Male");
            parsePart(maleNode, "Armor", actionSet.MaleArmor, actionSet.MaleArmorByCode);
            parsePart(maleNode, "Head", actionSet.MaleHead, actionSet.MaleHeadByCode);
            parsePart(maleNode, "Mantle", actionSet.MaleMantle, actionSet.MaleMantleByCode);
            parsePart(maleNode, "Weapon", actionSet.MaleWeapon, actionSet.MaleWeaponByCode);
            #endregion

            #region Female
            XElement femaleNode = xmlNode.Element("Female");
            parsePart(femaleNode, "Armor", actionSet.FemaleArmor, actionSet.FemaleArmorByCode);
            parsePart(femaleNode, "Head", actionSet.FemaleHead, actionSet.FemaleHeadByCode);
            parsePart(femaleNode, "Mantle", actionSet.FemaleMantle, actionSet.FemaleMantleByCode);
            parsePart(femaleNode, "Weapon", actionSet.FemaleWeapon, actionSet.FemaleWeaponByCode);
            #endregion

            #region Rider
            XElement riderNode = xmlNode.Element("Rider");
            if (riderNode != null)
            {
                foreach (XElement data in riderNode.Elements("Set"))
                {
                    int code = int.Parse(data.Attribute("Code").Value);
                    string id = data.Attribute("ID").Value;
                    string bundleDir = data.Attribute("BundleDir").Value;
                    actionSet.Rider[id] = new Component()
                    {
                        Code = code,
                        ID = id,
                        BundleDir = bundleDir,
                    };
                    actionSet.RiderByCode[code] = actionSet.Rider[id];
                }
            }
            #endregion

            return actionSet;
        }
    }
}
