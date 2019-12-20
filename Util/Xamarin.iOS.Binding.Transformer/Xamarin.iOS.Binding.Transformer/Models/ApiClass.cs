﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Xamarin.iOS.Binding.Transformer.Attributes;
using Xamarin.iOS.Binding.Transformer.Models.Collections;

namespace Xamarin.iOS.Binding.Transformer
{
    [XmlRoot(ElementName = "class")]
    public class ApiClass : ApiObject
    {
        [ChangeIgnore]
        protected internal override string NodeName => $"class[@name='{NativeName}']";

        [ChangeIgnore]
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public ApiTypeType Type { get; set; }

        [XmlAttribute(AttributeName = "disabledefaultctor")]
        public bool DisableDefaultCtor { get; set; }

        [XmlAttribute(AttributeName = "category")]
        public bool IsCategory { get; set; }

        [XmlAttribute(AttributeName = "protocol")]
        public bool IsProtocol { get; set; }


        private string _protocolName;

        [XmlAttribute(AttributeName = "protocolname")]
        public string ProtocolName
        {
            get { return _protocolName; }
            set 
            {
                _protocolName = value; 


            }
        }

        [XmlAttribute(AttributeName = "static")]
        public bool IsStatic { get; set; }

        [XmlAttribute(AttributeName = "partial")]
        public bool IsPartial { get; set; }

        [XmlAttribute(AttributeName = "advice")]
        public string Advice { get; set; }

        [XmlAttribute(AttributeName = "obsolete")]
        public string Obsolete { get; set; }

        [ChangeIgnore]
        [XmlElement(ElementName = "model", Order = 0)]
        public ApiTypeModel Model { get; set; }

        [ChangeIgnore]
        [XmlElement(ElementName = "basetype", Order = 1)]
        public ApiBaseType BaseType { get; set; }

        [ChangeIgnore]
        [XmlElement(ElementName = "implements", Order = 2)]
        public List<ApiImplements> Implements { get; set; }

        [ChangeIgnore]
        [XmlElement(ElementName = "property", Order = 3)]
        public List<ApiProperty> Properties { get; set; }

        [ChangeIgnore]
        [XmlElement(ElementName = "method", Order = 4)]
        public List<ApiMethod> Methods { get; set; }

        [ChangeIgnore]
        [XmlElement(ElementName = "verify", Order = 5)]
        public ApiVerify Verify { get; set; }

        #region Ignoreable Properties

        [XmlIgnore]
        private string NativeName
        {
            get
            {
                if (BaseType != null && !string.IsNullOrWhiteSpace(BaseType.Name))
                    return BaseType.Name;

                if (!string.IsNullOrWhiteSpace(ProtocolName))
                    return ProtocolName;

                return Name;
            }
        }

        #endregion

        #region Redirected Properties

        /// <summary>
        /// The base type - redirects to BaseType.TypeName
        /// </summary>
        [XmlIgnore]
        public string TypeName
        {
            get { return BaseType?.TypeName; }
            set 
            {
                if (BaseType == null)
                    BaseType = new ApiBaseType();

                BaseType.TypeName = value; 
            }
        }

        /// <summary>
        /// The Managed Name - redirects to BaseType.Name
        /// </summary>
        [XmlIgnore]
        public string ManagedName
        {
            get { return Name; }
            set
            {
                

                if ((string.IsNullOrWhiteSpace(BaseType?.Name) || !Name.Equals(value)))
                {
                    if (string.IsNullOrWhiteSpace(ProtocolName))
                    {
                        if (BaseType == null)
                            BaseType = new ApiBaseType();

                        BaseType.Name = Name;
                    }
                    else
                    {
                        ProtocolName = Name;
                    }
                    
                }


                Name = value;
                
            }
        }

        /// <summary>
        /// The EventsType value - redirects to BaseType.EventsType
        /// </summary>
        [XmlIgnore]
        public string EventsType
        {
            get { return BaseType?.EventsType; }
            set
            {
                if (BaseType == null)
                    BaseType = new ApiBaseType();

                BaseType.EventsType = value;
            }
        }

        /// <summary>
        /// The DelegateName value - redirects to BaseType.DelegateName
        /// </summary>
        [XmlIgnore]
        public string DelegateName
        {
            get { return BaseType?.DelegateName; }
            set
            {
                if (BaseType == null)
                    BaseType = new ApiBaseType();

                BaseType.DelegateName = value;
            }
        }

        /// <summary>
        /// The AutoGeneratedName value - redirects to Model.AutoGeneratedName
        /// </summary>
        [XmlIgnore]
        public bool AutoGeneratedName
        {
            get 
            {
                if (Model == null)
                    return false;

                return Model.AutoGeneratedName;
            }
            set
            {
                if (Model == null)
                    Model = new ApiTypeModel();

                Model.AutoGeneratedName = value;
            }
        }

        /// <summary>
        /// The VerifyType value - redirects to Verify.VerifyType
        /// </summary>
        [XmlIgnore]
        public string VerifyType
        {
            get { return Verify?.VerifyType; }
            set
            {
                if (Verify == null)
                    Verify = new ApiVerify();

                Verify.VerifyType = value;
            }
        }

        /// <summary>
        /// Comma seperated list of Inherited classes and interfaces - Redirects to Implements
        /// </summary>
        [XmlIgnore]
        public string InheritsFrom
        {
            get
            {
                if (Implements == null || Implements.Count == 0)
                    return null;

                var names = Implements.Select(x => x.Name);

                return string.Join(',', names);
            }
            set
            {
                Implements = new List<ApiImplements>();

                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                var names = value.Split(',').ToList();

                names.ForEach(x => Implements.Add(new ApiImplements
                {
                    Name = x,
                }));

            }
        }
        #endregion
        public ApiClass()
        {
            Implements = new List<ApiImplements>();
            Methods = new List<ApiMethod>();
            Properties = new List<ApiProperty>();
        }

        internal protected override void SetParent(ApiObject parent)
        {
            base.SetParentInternal(parent);

            foreach (var aObject in Methods)
            {
                aObject.SetParent(this);
            }

            foreach (var aObject in Properties)
            {
                aObject.SetParent(this);
            }
        }

        internal protected override void UpdatePathList(ref Dictionary<string, ApiObject> dict)
        {
            if (!dict.ContainsKey(Path))
                dict.Add(Path, this);

            foreach (var aNamespace in Methods)
            {
                aNamespace.UpdatePathList(ref dict);
            }

            foreach (var aNamespace in Properties)
            {
                aNamespace.UpdatePathList(ref dict);
            }
        }

        internal override void Add(ApiObject item)
        {
            if (item is ApiProperty)
            {
                Properties.Add((ApiProperty)item);
            }
            else if (item is ApiMethod)
            {
                var aItem = (ApiMethod)item;

                var propPath = aItem.GetProposedPath(this);

                var existingSigs = Methods.Where(x => x.Path.Equals(propPath, StringComparison.OrdinalIgnoreCase));

                if (existingSigs.Any())
                {
                    Console.WriteLine($"Class already has a method matching the signature: {aItem.ExportName}");
                }
                else
                {
                    Methods.Add(aItem);
                }
                
            }
        }

        internal override void Remove(ApiObject item)
        {
            if (item is ApiProperty)
            {
                Properties.Remove((ApiProperty)item);
            }
            else if (item is ApiMethod)
            {
                Methods.Remove((ApiMethod)item);
            }
        }

        internal void Merge(ApiClass additional)
        {
            Properties.AddRange(additional.Properties);
            Methods.AddRange(additional.Methods);
        }

        internal ApiClass Clone()
        {
            var newClass = (ApiClass)this.MemberwiseClone();

            

            return newClass;
        }

        public override void RemovePrefix(string prefix)
        {
            if (Name.StartsWith(prefix))
            {
                if (IsProtocol)
                {
                    ProtocolName = Name;

                    BaseType = null;
                }
                else
                {
                    if (BaseType == null)
                        BaseType = new ApiBaseType();

                    BaseType.Name = Name;
                }

                Name = Name.Replace(prefix, "");
            }

            foreach (var property in Properties)
            {
                property.RemovePrefix(prefix);
            }

            foreach (var method in Methods)
            {
                method.RemovePrefix(prefix);
            }
        }
    }
}