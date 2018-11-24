﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;

namespace Base.Common
{
    /// <summary>
    /// Xml文件解析类
    /// </summary>
    public class XMLHelper
    {
        /// <summary>
        /// 获取XmlDocument
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XmlDocument GetDocument(string path)
        {
            XmlDocument doc = new XmlDocument();
#if WINDOWS_UWP
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            doc.Load(stream);
#else
            doc.Load(path);
#endif
            return doc;
        }
#if WINDOWS_UWP
        public static XmlNode SelectSingleNode(this XmlDocument doc,string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            string[] parts = path.Split('/');
            if (parts.Length == 1)
            {
                XmlNodeList nodeList = doc.GetElementsByTagName(parts[0]);
                if (nodeList == null || nodeList.Count == 0) return null;
                return nodeList[0];
            }
            else
            {
                XmlNodeList nodeList = doc.GetElementsByTagName(parts[0]);
                if (nodeList == null || nodeList.Count == 0) return null;
                return SelectSingleNode(path, nodeList);
            }
        }

        public static XmlNode SelectSingleNode(this XmlNode rootNode, string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
           

            XmlNodeList nodeList = rootNode.ChildNodes;
            return SelectSingleNode(path, nodeList);
        }

        private static XmlNode SelectSingleNode(string path, XmlNodeList nodeList)
        {
            if (nodeList == null || nodeList.Count == 0) return null;

            string[] parts = path.Split('/');
            List<XmlNode> result = new List<XmlNode>();
            foreach (XmlNode node in nodeList)
            {
                result.Add(node);
            }

            for (int i = 1; i < parts.Length; i++)
            {
                result = result.GetElementsByTagName(parts[i]);
            }

            if (result == null || result.Count == 0) return null;
            return nodeList[0];
        }

        public static List<XmlNode> GetElementsByTagName(this List<XmlNode> nodes, string nodeName)
        {
            List<XmlNode> result = new List<XmlNode>();
            foreach (XmlNode node2 in result)
            {
                result.AddRange(node2.GetElementsByTagName(nodeName));
            }
            return result;
        }

        public static List<XmlNode> GetElementsByTagName(this XmlNode node1, string nodeName)
        {
            List<XmlNode> nodes = new List<XmlNode>();
            foreach (XmlNode node2 in node1.ChildNodes)
            {
                if (node2.Name == nodeName)
                {
                    nodes.Add(node2);
                }
            }
            return nodes;
        }
#endif

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="strpath"></param>
        public XMLHelper(string strpath)
        {
            FilePath = strpath;
            OpenXML();
        }

        #region 对象定义
        private XmlDocument xmlDoc = new XmlDocument();
        XmlNode xmlnode;
        XmlElement xmlelem;
        #endregion
        #region 属性定义
        private string errorMess;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMess
        {
            get { return errorMess; }
            set { errorMess = value; }
        }
        private string filePath;
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath
        {
            set { filePath = value; }
            get { return filePath; }
        }
        #endregion
        #region 创建XML操作对象
        /// <summary>
        /// 创建XML操作对象
        /// </summary>
        public void OpenXML()
        {
            try
            {
                if (!string.IsNullOrEmpty(FilePath))
                {
                    xmlDoc.Load(filePath);
                }
                //else
                //{
                //    FilePath = System.Windows.Forms.Application.StartupPath + string.Format(@"\IomViewCli.config"); //默认路径
                //    xmlDoc.Load(filePath);
                //}
            }
            catch (Exception ex)
            {
                ErrorMess = ex.Message;
            }
        }
        #endregion
        #region 创建Xml 文档
        /// <summary>
        /// 创建一个带有根节点的Xml 文件
        /// </summary>
        /// <param name="FileName">Xml 文件名称</param>
        /// <param name="rootName">根节点名称</param>
        /// <param name="Encode">编码方式:gb2312，UTF-8 等常见的</param>
        /// <returns></returns>
        public bool CreatexmlDocument(string FileName, string rootName, string Encode)
        {
            try
            {
                XmlDeclaration xmldecl;
                xmldecl = xmlDoc.CreateXmlDeclaration("1.0", Encode, null);
                xmlDoc.AppendChild(xmldecl);
                xmlelem = xmlDoc.CreateElement("", rootName, "");
                xmlDoc.AppendChild(xmlelem);
                xmlDoc.Save(FileName);
                return true;
            }
            catch (Exception e)
            {
                Log.Error("XmlHelper.CreatexmlDocument", e);
                return false;
            }
        }
        #endregion
        //获取值
        #region 得到表
        /// <summary>
        /// 得到表
        /// </summary>
        /// <returns></returns>
        public DataView GetData()
        {
            xmlDoc = new XmlDocument();
            DataSet ds = new DataSet();
            StringReader read = new StringReader(xmlDoc.SelectSingleNode(FilePath).OuterXml);
            ds.ReadXml(read);
            return ds.Tables[0].DefaultView;
        }
        #endregion
        #region 读取指定节点的指定属性值
        /// <summary>
        /// 功能:
        /// 读取指定节点的指定属性值
        /// </summary>
        /// <param name="strNode">节点名称(相对路径：//+节点名称)</param>
        /// <param name="strAttribute">此节点的属性</param>
        /// <returns></returns>
        public string GetXmlNodeValue(string strNode, string strAttribute)
        {
            string strReturn = "";
            try
            {
                //根据指定路径获取节点
                XmlNode xmlNode = xmlDoc.SelectSingleNode(strNode);
                if (xmlNode == null)
                {
                    Log.Error("XmlHelper.GetXmlNodeValue", "未找到节点:" + strNode);
                    return strReturn;
                }
                //获取节点的属性，并循环取出需要的属性值
                XmlAttributeCollection xmlAttr = xmlNode.Attributes;
                for (int i = 0; i < xmlAttr.Count; i++)
                {
                    if (xmlAttr.Item(i).Name == strAttribute)
                    {
                        strReturn = xmlAttr.Item(i).Value;
                    }
                }
            }
            catch (XmlException xmle)
            {
                throw xmle;
            }
            return strReturn;
        }

        /// <summary>
        /// 根据路径查找是否存在该节点
        /// </summary>
        /// <param name="szPath"></param>
        /// <returns></returns>
        public bool FindNodeByPath(string szPath)
        {
            try
            {
                XmlNode xmlNode = xmlDoc.SelectSingleNode(szPath);
                if (xmlNode != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        /// <param name="strNode"></param>
        /// <returns></returns>
        public XmlNode GetXmlNode(string strNode)
        {
            XmlNode xmlNode = null;
            try
            {
                xmlNode = xmlDoc.SelectSingleNode(strNode);
            }
            catch (XmlException xmle)
            {
                throw xmle;
            }
            return xmlNode;
        }

        #endregion
        #region 读取指定节点的值
        /// <summary>
        /// 功能:
        /// 读取指定节点的值
        /// </summary>
        /// <param name="strNode">节点名称</param>
        /// <returns></returns>
        public string GetXmlNodeValue(string strNode)
        {
            string strReturn = String.Empty;
            try
            {
                //根据路径获取节点
                XmlNode xmlNode = xmlDoc.SelectSingleNode(strNode);
                if (xmlNode == null)
                {
                    return strReturn;
                }
                else
                {
                    strReturn = xmlNode.InnerText;
                }
            }
            catch (XmlException xmle)
            {
                System.Console.WriteLine(xmle.Message);
            }
            return strReturn;
        }
        #endregion
        #region 获取XML文件的根元素
        /// <summary>
        /// 获取XML文件的根元素
        /// </summary>
        public XmlNode GetXmlRoot()
        {
            return xmlDoc.DocumentElement;
        }
        #endregion
        #region 获取XML节点值
        /// <summary>
        /// 获取XML节点值
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public string GetNodeValue(string nodeName)
        {
            xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);
            XmlNodeList xnl = xmlDoc.ChildNodes;
            foreach (XmlNode xnf in xnl)
            {
                XmlElement xe = (XmlElement)xnf;
                XmlNodeList xnf1 = xe.ChildNodes;
                foreach (XmlNode xn2 in xnf1)
                {
                    if (xn2.InnerText == nodeName)
                    {
                        XmlElement xe2 = (XmlElement)xn2;
                        return xe2.GetAttribute("value");
                    }
                }
            }
            return null;
        }
        #endregion
        //添加或插入

        /// <summary>
        /// 设置节点值
        /// </summary>
        /// <param name="path">节点的路径</param>
        /// <param name="value">节点值</param>
        public void SetXmlNodeValue(string path, string value)
        {
            try
            {
                //根据指定路径获取节点
                XmlNode xmlNode = xmlDoc.SelectSingleNode(path);
                //设置节点值
                xmlNode.InnerText = value;
                SavexmlDocument();
            }
            catch (XmlException xmle)
            {
                throw xmle;
            }
        }
        #region 添加父节点
        /// <summary>
        /// 在根节点下添加父节点
        /// </summary>
        public void AddParentNode(string parentNode)
        {
            XmlNode root = GetXmlRoot();
            XmlNode parentXmlNode = xmlDoc.CreateElement(parentNode);
            root.AppendChild(parentXmlNode);
        }
        #endregion
        #region 向一个已经存在的父节点中插入一个子节点
        /// <summary>
        /// 向一个已经存在的父节点中插入一个子节点
        /// </summary>
        public void AddChildNode(string parentNodePath, string childNodePath)
        {
            XmlNode parentXmlNode = xmlDoc.SelectSingleNode(parentNodePath);
            XmlElement childXmlNode = xmlDoc.CreateElement(childNodePath);
            parentXmlNode.AppendChild(childXmlNode);
        }

        #endregion
        #region 向一个节点添加属性
        /// <summary>
        /// 向一个节点添加属性
        /// </summary>
        public void AddAttribute(string NodePath, string NodeAttribute)
        {
            XmlAttribute nodeAttribute = xmlDoc.CreateAttribute(NodeAttribute);
            XmlNode nodePath = xmlDoc.SelectSingleNode(NodePath);
            nodePath.Attributes.Append(nodeAttribute);
        }
        #endregion
        #region 插入一个节点和它的若干子节点
        /// <summary>
        /// 插入一个节点和它的若干子节点
        /// </summary>
        /// <param name="NewNodeName">插入的节点名称</param>
        /// <param name="HasAttributes">此节点是否具有属性，True 为有，False 为无</param>
        /// <param name="fatherNode">此插入节点的父节点</param>
        /// <param name="htAtt">此节点的属性，Key 为属性名，Value 为属性值</param>
        /// <param name="htSubNode"> 子节点的属性， Key 为Name,Value 为InnerText</param>
        /// <returns>返回真为更新成功，否则失败</returns>
        public bool InsertNode(string NewNodeName, bool HasAttributes, string fatherNode, Hashtable htAtt, Hashtable htSubNode)
        {
            try
            {
                xmlDoc.Load(FilePath);
                XmlNode root = xmlDoc.SelectSingleNode(fatherNode);
                xmlelem = xmlDoc.CreateElement(NewNodeName);
                if (htAtt != null && HasAttributes)//若此节点有属性，则先添加属性
                {
                    SetAttributes(xmlelem, htAtt);
                    AddNodes(xmlelem.Name, xmlDoc, xmlelem, htSubNode);//添加完此节点属性后，再添加它的子节点和它们的InnerText
                }
                else
                {
                    AddNodes(xmlelem.Name, xmlDoc, xmlelem, htSubNode);//若此节点无属性，那么直接添加它的子节点
                }
                root.AppendChild(xmlelem);
                xmlDoc.Save(FilePath);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion
        #region 设置节点属性
        /// <summary>
        /// 设置节点属性
        /// </summary>
        /// <param name="xe">节点所处的Element</param>
        /// <param name="htAttribute">节点属性，Key 代表属性名称，Value 代表属性值</param>
        public void SetAttributes(XmlElement xe, Hashtable htAttribute)
        {
            foreach (DictionaryEntry de in htAttribute)
            {
                xe.SetAttribute(de.Key.ToString(), de.Value.ToString());
            }
        }
        #endregion
        #region 增加子节点到根节点下
        /// <summary>
        /// 增加子节点到根节点下
        /// </summary>
        /// <param name="rootNode">上级节点名称</param>
        /// <param name="xmlDoc">Xml 文档</param>
        /// <param name="rootXe">父根节点所属的Element</param>
        /// <param name="SubNodes">子节点属性，Key 为Name 值，Value 为InnerText 值</param>
        public void AddNodes(string rootNode, XmlDocument xmlDoc, XmlElement rootXe, Hashtable SubNodes)
        {
            if (SubNodes != null)
            {
                foreach (DictionaryEntry de in SubNodes)
                {
                    xmlnode = xmlDoc.SelectSingleNode(rootNode);
                    XmlElement subNode = xmlDoc.CreateElement(de.Key.ToString());
                    subNode.InnerText = de.Value.ToString();
                    rootXe.AppendChild(subNode);
                }
            }
        }
        #endregion
        //更新
        #region 设置节点的属性值
        /// <summary>
        /// 功能:
        /// 设置节点的属性值
        /// </summary>
        /// <param name="xmlNodePath">节点名称</param>
        /// <param name="xmlNodeAttribute">属性名称</param>
        /// <param name="xmlNodeAttributeValue">属性值</param>
        public void SetXmlNodeValue(string xmlNodePath, string xmlNodeAttribute, string xmlNodeAttributeValue)
        {
            try
            {
                //根据指定路径获取节点
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xmlNodePath);
                //获取节点的属性，并循环取出需要的属性值
                XmlAttributeCollection xmlAttr = xmlNode.Attributes;
                for (int i = 0; i < xmlAttr.Count; i++)
                {
                    if (xmlAttr.Item(i).Name == xmlNodeAttribute)
                    {
                        xmlAttr.Item(i).Value = xmlNodeAttributeValue;
                        break;
                    }
                }
                SavexmlDocument();
            }
            catch (XmlException xmle)
            {
                throw xmle;
            }
        }
        #endregion
        #region 更新节点
        /// <summary>
        /// 更新节点
        /// </summary>
        /// <param name="fatherNode">需要更新节点的上级节点</param>
        /// <param name="htAtt">需要更新的属性表，Key 代表需要更新的属性，Value 代表更新后的值</param>
        /// <param name="htSubNode">需要更新的子节点的属性表，Key 代表需要更新的子节点名字Name,Value 代表更新后的值InnerText</param>
        /// <returns>返回真为更新成功，否则失败</returns>
        public bool UpdateNode(string fatherNode, Hashtable htAtt, Hashtable htSubNode)
        {
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(FilePath);
                XmlNodeList root = xmlDoc.SelectSingleNode(fatherNode).ChildNodes;
                UpdateNodes(root, htAtt, htSubNode);
                xmlDoc.Save(FilePath);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion
        #region 更新节点属性和子节点InnerText 值
        /// <summary>
        /// 更新节点属性和子节点InnerText 值
        /// </summary>
        /// <param name="root">根节点名字</param>
        /// <param name="htAtt">需要更改的属性名称和值</param>
        /// <param name="htSubNode">需要更改InnerText 的子节点名字和值</param>
        public void UpdateNodes(XmlNodeList root, Hashtable htAtt, Hashtable htSubNode)
        {
            foreach (XmlNode xn in root)
            {
                xmlelem = (XmlElement)xn;
                if (xmlelem.HasAttributes)//如果节点如属性，则先更改它的属性
                {
                    foreach (DictionaryEntry de in htAtt)//遍历属性哈希表
                    {
                        if (xmlelem.HasAttribute(de.Key.ToString()))//如果节点有需要更改的属性
                        {
                            xmlelem.SetAttribute(de.Key.ToString(), de.Value.ToString());//则把哈希表中相应的值Value 赋给此属性Key
                        }
                    }
                }
                if (xmlelem.HasChildNodes)//如果有子节点，则修改其子节点的InnerText
                {
                    XmlNodeList xnl = xmlelem.ChildNodes;
                    foreach (XmlNode xn1 in xnl)
                    {
                        XmlElement xe = (XmlElement)xn1;
                        foreach (DictionaryEntry de in htSubNode)
                        {
                            if (xe.Name == de.Key.ToString())//htSubNode 中的key 存储了需要更改的节点名称，
                            {
                                xe.InnerText = de.Value.ToString();//htSubNode中的Value存储了Key 节点更新后的数据
                            }
                        }
                    }
                }
            }
        }
        #endregion
        //删除
        #region 删除一个节点的属性
        /// <summary>
        /// 删除一个节点的属性
        /// </summary>
        public void DeleteAttribute(string NodePath, string NodeAttribute, string NodeAttributeValue)
        {
            XmlNodeList nodePath = xmlDoc.SelectSingleNode(NodePath).ChildNodes;
            foreach (XmlNode xn in nodePath)
            {
                XmlElement xe = (XmlElement)xn;
                if (xe.GetAttribute(NodeAttribute) == NodeAttributeValue)
                {
                    xe.RemoveAttribute(NodeAttribute);//删除属性
                }
            }
        }
        #endregion
        #region 删除一个节点
        /// <summary>
        /// 删除一个节点
        /// </summary>
        public void DeleteXmlNode(string tempXmlNode)
        {
            XmlNode xmlNodePath = xmlDoc.SelectSingleNode(tempXmlNode);
            if (xmlNodePath != null)
            {
                xmlNodePath.ParentNode.RemoveChild(xmlNodePath);
            }
            SavexmlDocument();
        }
        #endregion
        #region 删除指定节点下的子节点
        /// <summary>
        /// 删除指定节点下的子节点
        /// </summary>
        /// <param name="fatherNode">制定节点</param>
        /// <returns>返回真为更新成功，否则失败</returns>
        public bool DeleteNodes(string fatherNode)
        {
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(FilePath);
                xmlnode = xmlDoc.SelectSingleNode(fatherNode);
                if (xmlnode != null)
                {
                    xmlnode.RemoveAll();
                    xmlDoc.Save(FilePath);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (XmlException xe)
            {
                throw new XmlException(xe.Message);
            }
        }
        #endregion
        //内部函数与保存
        #region 私有函数
        private string functionReturn(XmlNodeList xmlList, int i, string nodeName)
        {
            string node = xmlList[i].ToString();
            string rusultNode = "";
            for (int j = 0; j < i; j++)
            {
                if (node == nodeName)
                {
                    rusultNode = node.ToString();
                }
                else
                {
                    if (xmlList[j].HasChildNodes)
                    {
                        functionReturn(xmlList, j, nodeName);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return rusultNode;

        }
        #endregion
        #region 保存XML文件
        /// <summary>
        /// 功能: 保存XML文件
        /// </summary>
        public void SavexmlDocument()
        {
            try
            {
                xmlDoc.Save(FilePath);
            }
            catch (XmlException xmle)
            {
                throw xmle;
            }
        }
        /// <summary>
        /// 功能: 保存XML文件
        /// </summary>
        /// <param name="tempXMLFilePath"></param>
        public void SavexmlDocument(string tempXMLFilePath)
        {
            try
            {
                xmlDoc.Save(tempXMLFilePath);
            }
            catch (XmlException xmle)
            {
                throw xmle;
            }
        }
        #endregion
    }
}
