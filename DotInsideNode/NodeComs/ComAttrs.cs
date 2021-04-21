using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotInsideNode
{
    /// <summary>
    /// A component only conenect one component
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    class SingleConnect:System.Attribute
    {
        public void Process(LinkManager linkManager, INodeInput inCom)
        {
            if (linkManager.RemoveLinkByEnd(inCom.ID))
            {
                Logger.Info(inCom.GetType().Name + " is SingleConncect");
            }
        }

        public void Process(LinkManager linkManager, INodeOutput outCom)
        {
            if (linkManager.RemoveLinkByBegin(outCom.ID))
            {
                Logger.Info(outCom.GetType().Name + " is SingleConncect");
            }
        }
    }

    /// <summary>
    /// Component could connect component types
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    class ConnectTypes : System.Attribute
    {
        HashSet<Type> m_Types = new HashSet<Type>();

        public ConnectTypes(params Type[] types)
        {
            foreach(Type type in types)
            {
                m_Types.Add(type);
            }
        }

        public bool Contains(Type type)
        {
            return m_Types.Contains(type);
        }
    }

    /// <summary>
    /// Component could connect component types
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    class ConnectValueType : System.Attribute
    {
        Type m_Type = null;

        //All diType Class Instance
        public static Dictionary<Type,Attribute> ConvertNdoeDict = new Dictionary<Type, Attribute>();

        public static void InitClassList()
        {
            if (ConvertNdoeDict.Count != 0)
                return;

            ConvertNdoeDict = AttributeTools.GetNamespaceCustomAttributes(typeof(AConvertNode));
            foreach (var node in ConvertNdoeDict)
                Logger.Info("ConvertNdoe Type: " + node.Key);
        }

        public ConnectValueType(Type accept_type)
        {
            m_Type = accept_type;
        }

        public bool Process(INodeGraph nodeGraph, INodeInput inCom, INodeOutput outCom)
        {
            Type outValueType = (Type)outCom.Request(ERequest.InstanceType);
            if (outValueType.Equals(AcceptType))
            {
                return true;
            }
            else
            {
                Logger.Info(outValueType.Name + " need convert to: " + AcceptType.Name);
                TryAddConvertNdoe(nodeGraph, inCom, outCom,outValueType);
                return false;
            }
        }

        public void TryAddConvertNdoe(INodeGraph nodeGraph, INodeInput inCom, INodeOutput outCom,Type outValueType)
        {
            foreach(var nodePair in ConvertNdoeDict)
            {
                AConvertNode aConvert = (AConvertNode)nodePair.Value;
                if (aConvert.ConvertType.Equals(AcceptType))
                {
                    IConvertNode convertNode = (IConvertNode)ClassTools.CallConstructor(nodePair.Key, nodeGraph);
                    AddNodeBetween(nodeGraph, inCom.ParentNode, outCom.ParentNode, convertNode);
                    convertNode.InputConnect(outCom);
                    convertNode.OutputConnect(inCom);
                }
            }
        }

        public void AddNodeBetween(INodeGraph nodeGraph, INode left, INode right,INode newNode)
        {
            nodeGraph.ngNodeManager.AddNode(newNode, false);

            //if (nodeGraph.O)
            {
                //Set Position
                Vector2 lPos = nodeGraph.ngNodeManager.GetNodeEditorPostion(left);
                Vector2 rPos = nodeGraph.ngNodeManager.GetNodeEditorPostion(right);
                Vector2 newPos = new Vector2((lPos.x + rPos.x) * 0.5f, (lPos.y + rPos.y) * 0.5f);

                nodeGraph.ngNodeManager.SetNodeEditorPostion(newNode, newPos);
            }
        }

        public Type AcceptType => m_Type;
    }

    [AttributeUsage(AttributeTargets.Class)]
    class AConvertNode : System.Attribute
    {
        Type m_Type = null;

        public AConvertNode(Type convert_to)
        {
            m_Type = convert_to;
        }

        public Type ConvertType => m_Type;
    }

}
