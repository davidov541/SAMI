using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using SAMI.Configuration;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Remote
{
    [ParseableElement("RemoteCode", ParseableElementType.Support)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class RemoteCode : IParseable
    {
        public String Name
        {
            get;
            private set;
        }

        public byte[] Code
        {
            get;
            private set;
        }

        private String CodeString
        {
            get
            {
                String str = String.Empty;
                foreach (byte b in Code)
                {
                    str += b.ToString("x") + "-";
                }
                str.Substring(0, str.Length - 1);
                return str;
            }
            set
            {
                byte[] codes = new byte[value.Count(c => c == '-') + 1];
                int i = 0;
                foreach (String codePortion in value.Split('-'))
                {
                    codes[i++] = byte.Parse(codePortion, NumberStyles.AllowHexSpecifier);
                }
                Code = codes;
            }
        }

        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public IEnumerable<PersistentProperty> Properties
        {
            get 
            {
                yield return new PersistentProperty("Name", () => Name, n => Name = n);
                yield return new PersistentProperty("Code", () => CodeString, cs => CodeString = cs);
            }
        }

        public IEnumerable<IParseable> Children
        {
            get 
            {
                yield break;
            }
        }

        public RemoteCode()
        {
        }

        public void Initialize(IConfigurationManager manager)
        {
        }

        public void AddChild(IParseable child)
        {
        }

        public void Dispose()
        {
        }
    }
}
