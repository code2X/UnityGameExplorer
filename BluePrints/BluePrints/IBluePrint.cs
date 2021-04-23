
using System;
using System.Collections.Generic;

namespace DotInsideNode
{

    [Serializable]
    public abstract class IBluePrint: dnObject 
    {
        //All diType Class Instance
        public static List<IBluePrint> TypeClassList = new List<IBluePrint>();

        public static void InitClassList()
        {
            if (TypeClassList.Count != 0)
                return;

            TypeClassList = AttributeTools.GetCustomAttributeClassList<IBluePrint>(typeof(BlueprintClass));
            foreach (var container in TypeClassList)
                Logger.Info("BluePrint Class: " + container);
        }

        public abstract void Draw();

        //For judge blueprint is comileable
        public virtual bool Compileable
        {
            get => false;
        }
        public virtual void Compile() { }
        public virtual void Save(string dirPath) { }

        //When create a new instance, need default base name to create file
        public abstract string NewBaseName
        {
            get;
        }
    }
}