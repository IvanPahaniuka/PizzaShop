using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.CustomSerializer
{
    public static class TypeExtention
    {
        public static bool IsStruct(this Type type) =>
            type.IsValueType &&
            !type.IsPrimitive &&
            !type.IsEnum &&
            !type.IsEquivalentTo(typeof(string));

        public static object DefaultValue(this Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }
    }

    public class CustomSerializer
    {
        private class ObjectDescriptor
        {
            public int Index;
            public object Obj;
            public DataObject DataObject;
            public List<Data> RefDatas = new List<Data>();
            public List<string> Arrays = new List<string>();
        }

        private class SerializerObject
        {
            public int Index;
            public object Obj;
        }

        private class DataObject
        {
            private List<Data> datas;

            public List<Data> Datas
            {
                get => datas ?? (datas = new List<Data>());
                set => datas = value;
            }
        }

        private class Data
        {
            public string PropertyName;
            public string Value;
        }


        public void Serialize<T>(Stream stream, T obj) where T : class
        {
            using (StreamWriter SW = new StreamWriter(stream))
            {
                SW.Write(Serialize(obj));
            }
        }

        public T Deserialize<T>(Stream stream) where T : class
        {
            T res;
            using (StreamReader SR = new StreamReader(stream))
            {
                string str = SR.ReadToEnd();
                res = Deserialize<T>(str);
            }
            return res;
        }



        public string Serialize(object obj)
        {
            return Serialize(obj, new List<SerializerObject>());
        }

        private string Serialize(object obj, List<SerializerObject> objects)
        {

            var objectsToAdd = new List<string>();
            var sb = new StringBuilder();
            sb.Append("{");
            sb.AppendLine();
            if (obj == null)
            {
                sb.Append("\t#Type:\"null\";");
                sb.AppendLine();
                sb.Append($"\t#Id:\"#{objects.Count}\";");
                sb.AppendLine();
                objects.Add(new SerializerObject { Index = objects.Count, Obj = obj });
                sb.Append("}");
                return sb.ToString();
            }
            sb.Append($"\t#Type:\"{obj.GetType().AssemblyQualifiedName}\";");
            sb.AppendLine();
            sb.Append($"\t#Id:\"#{objects.Count}\";");
            sb.AppendLine();
            objects.Add(new SerializerObject { Index = objects.Count, Obj = obj });

            var myType = obj.GetType();
            if (myType.GetCustomAttribute<NonSerializedAttribute>() != null ||
                myType.GetCustomAttribute<SerializableAttribute>() == null)
                throw new Exception("Тип не является Serializable");

            var props = new List<PropertyInfo>(myType.GetProperties());
            if (typeof(IDictionary).IsAssignableFrom(myType))
            {
                var dict = obj as IDictionary;
                foreach (var key in dict.Keys)
                {
                    sb.Append($"\tItem[{GetStringFromValue(objects, key, objectsToAdd)}]:{GetStringFromValue(objects, dict[key], objectsToAdd)};");
                    sb.AppendLine();
                }
            }
            else
            if (typeof(IList).IsAssignableFrom(myType))
            {
                int i = 0;
                var list = obj as IList;
                foreach (var element in list)
                {
                    sb.Append($"\tItem[\"{i}\"]:{GetStringFromValue(objects, element, objectsToAdd)};");
                    sb.AppendLine();
                    i++;
                }
            }
            else
                foreach (var prop in props)
                    if (prop.GetCustomAttribute<NonSerializedAttribute>() == null)
                    {
                        var indexParams = prop.GetIndexParameters();
                        if ((indexParams?.Count() ?? 0) == 0)
                        {
                            sb.Append($"\t{prop.Name}:{GetStringFromValue(objects, prop, obj, objectsToAdd)};");
                            sb.AppendLine();
                        }
                    }



            sb.AppendLine();
            sb.Append("}");

            foreach (var objToAdd in objectsToAdd)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(objToAdd);
            }

            return sb.ToString();
        }

        public T Deserialize<T>(string str) where T : class
        {
            var dataObjects = LoadDataObjects(str);
            var objects = ConvertToObjects(dataObjects);
            InitializeArrays(objects);
            LinkObjects(objects);

            return (T)objects[0].Obj;
        }

        private string GetStringFromValue(List<SerializerObject> objects, PropertyInfo prop, object obj, List<string> objectsToAdd)
        {
            var propValue = prop.GetValue(obj, null);
            if (typeof(Enum).IsAssignableFrom(prop.PropertyType))
            {
                return $"\"{ Enum.Format(prop.PropertyType, propValue, "d") }\"";
            }
            else
            if ((!prop.PropertyType.IsClass &&
                !prop.PropertyType.IsInterface &&
                !prop.PropertyType.IsStruct() &&
                !typeof(ICollection).IsAssignableFrom(prop.PropertyType) &&
                !typeof(IDictionary).IsAssignableFrom(prop.PropertyType)) ||
                prop.PropertyType.IsEquivalentTo(typeof(string)) ||
                prop.PropertyType.IsEquivalentTo(typeof(decimal)))
            {
                return $"\"{propValue}\"";
            }
            else
            {
                if (objects.FirstOrDefault(o => o.Obj == propValue) == null)
                {
                    objectsToAdd.Add(Serialize(propValue, objects));
                }
                var index = objects.FindIndex(s => s.Obj == propValue);
                return $"\"#{index}\"";
            }
        }

        private string GetStringFromValue(List<SerializerObject> objects, object value, List<string> objectsToAdd)
        {
            if (typeof(Enum).IsAssignableFrom(value.GetType()))
            {
                return $"\"{ Enum.Format(value.GetType(), value, "d") }\"";
            }
            else
            if ((!value.GetType().IsClass &&
                !value.GetType().IsInterface &&
                !value.GetType().IsStruct() &&
                !typeof(ICollection).IsAssignableFrom(value.GetType()) &&
                !typeof(IDictionary).IsAssignableFrom(value.GetType())) ||
                value.GetType().IsEquivalentTo(typeof(string)) ||
                value.GetType().IsEquivalentTo(typeof(decimal)))
            {
                return $"\"{value}\"";
            }
            else
            {
                if (objects.FirstOrDefault(o => o.Obj == value) == null)
                {
                    objectsToAdd.Add(Serialize(value, objects));
                }
                var index = objects.FindIndex(s => s.Obj == value);
                return $"\"#{index}\"";
            }
        }

        private void LinkObjects(IEnumerable<ObjectDescriptor> objects)
        {
            foreach (var obj in objects)
            {
                if (obj.Obj == null)
                    continue;

                var objType = obj.Obj.GetType();
                foreach (var data in obj.RefDatas)
                {
                    var propInfo = objType.GetProperty(data.PropertyName);
                    propInfo.SetValue(obj.Obj, GetValue(objects, propInfo.PropertyType, data.Value), null);
                }
            }
        }

        private void InitializeArrays(IEnumerable<ObjectDescriptor> objects)
        {
            foreach (var obj in objects)
            {
                if (obj.Obj == null)
                    continue;

                var dataObj = obj.DataObject;
                var objType = obj.Obj.GetType();

                if (typeof(IDictionary).IsAssignableFrom(objType))
                {
                    var objTyped = obj.Obj as IDictionary;

                    var arrDatas = dataObj.Datas.Where(d => GetArrayName(d.PropertyName) == "Item");

                    foreach (var arrData in arrDatas)
                    {
                        var propInfo = objType.GetProperty("Item");
                        if (propInfo != null)
                        {
                            var indexesInfo = propInfo.GetIndexParameters();
                            var index = GetValue(objects, indexesInfo[0].ParameterType, GetValueFromArrayName(arrData.PropertyName, 0));
                            objTyped.Add(index, GetValue(objects, propInfo.PropertyType, arrData.Value));

                        }
                    }
                }
                else
                if (typeof(IList).IsAssignableFrom(objType))
                {
                    var objTyped = obj.Obj as IList;

                    var arrDatas = dataObj.Datas
                        .Where(d => GetArrayName(d.PropertyName) == "Item")
                        .OrderBy(d => int.Parse(GetValueFromArrayName(d.PropertyName, 0)));

                    foreach (var arrData in arrDatas)
                    {
                        var propInfo = objType.GetProperty("Item");
                        if (propInfo != null)
                        {
                            objTyped.Add(GetValue(objects, propInfo.PropertyType, arrData.Value));
                        }
                    }
                }
            }
        }

        private string GetValueFromArrayName(string name, int index)
        {
            var val = new StringBuilder();
            var clearName = ReplaceStrings(name);
            var first = clearName.IndexOf('[');
            if (first < 0)
                throw new ArgumentException();
            int k = 0;


            for (int i = first + 1; (i < clearName.Length) && (k <= index); i++)
            {
                if (clearName[i] != ']')
                    if (clearName[i] == '[' || clearName[i] == ',')
                    {
                        k++;
                    }
                    else
                    {
                        if (k == index)
                            val.Append(name[i]);
                    }
            }

            return ClearValue(val.ToString());
        }

        private object GetValue(IEnumerable<ObjectDescriptor> objects, Type valueType, string value)
        {
            if (value.StartsWith("#"))
            {
                int id = int.Parse(value.TrimStart('#'));
                return objects.First(o => o.Index == id).Obj;
            }

            return Convert.ChangeType(value, valueType);
        }

        private List<ObjectDescriptor> ConvertToObjects(List<DataObject> dataObjects)
        {
            var dataObjectsCp = new List<DataObject>(dataObjects.Count);
            for (int i = 0; i < dataObjects.Count; i++)
                dataObjectsCp.Add(new DataObject { Datas = new List<Data>(dataObjects[i].Datas) });

            var objects = new List<ObjectDescriptor>(dataObjectsCp.Count);

            for (int i = 0; i < dataObjectsCp.Count; i++)
            {
                objects.Add(new ObjectDescriptor());
                var typeData = dataObjectsCp[i].Datas.First(d => d.PropertyName == "#Type");
                dataObjectsCp[i].Datas.Remove(typeData);
                var typeStr = Convert.ChangeType(typeData.Value, typeof(string)) as string;
                if (typeStr == "null")
                {
                    objects[i].Obj = null;
                    var idDataC = dataObjectsCp[i].Datas.First(d => d.PropertyName == "#Id");
                    dataObjectsCp[i].Datas.Remove(idDataC);
                    int idC = int.Parse(idDataC.Value.TrimStart('#'));
                    objects[i].Index = idC;
                    continue;
                }
                var objType = Type.GetType(
                    typeName: typeStr,
                    throwOnError: true,
                    ignoreCase: true);

                if (objType.GetCustomAttribute<SerializableAttribute>() == null ||
                    objType.GetCustomAttribute<NonSerializedAttribute>() != null)
                    continue;

                var obj = Activator.CreateInstance(objType);
                objects[i].Obj = obj;

                var idData = dataObjectsCp[i].Datas.First(d => d.PropertyName == "#Id");
                dataObjectsCp[i].Datas.Remove(idData);
                int id = int.Parse(idData.Value.TrimStart('#'));
                objects[i].Index = id;

                objects[i].DataObject = dataObjects[i];

                foreach (var data in dataObjectsCp[i].Datas)
                    if (!data.PropertyName.Contains('['))
                    {
                        var propInfo = objType.GetProperty(data.PropertyName);
                        if (propInfo.GetCustomAttribute<NonSerializedAttribute>() == null)
                        {
                            if ((!propInfo.PropertyType.IsClass &&
                                !propInfo.PropertyType.IsInterface &&
                                !propInfo.PropertyType.IsStruct()) ||
                                propInfo.PropertyType.IsEquivalentTo(typeof(string)) ||
                                propInfo.PropertyType.IsEquivalentTo(typeof(decimal)))
                            {
                                if (typeof(Enum).IsAssignableFrom(propInfo.PropertyType))
                                {
                                    propInfo?.SetValue(obj, Enum.Parse(propInfo.PropertyType, data.Value));
                                }
                                else
                                {
                                    propInfo?.SetValue(obj,
                                       Convert.ChangeType(data.Value, propInfo.PropertyType));
                                }

                            }
                            else
                            {
                                objects[i].RefDatas.Add(data);
                            }
                        }
                    }
                    else
                    {
                        var arrName = GetArrayName(data.PropertyName);
                        if (!objects[i].Arrays.Contains(arrName))
                            objects[i].Arrays.Add(arrName);
                    }
            }

            return objects;
        }

        private List<DataObject> LoadDataObjects(string str)
        {
            str = str.Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\t", string.Empty);
            var clearStr = ReplaceStrings(str);
            var objects = new List<DataObject>();
            var start = 0;

            while (start < str.Length && start >= 0)
            {
                var indexStart = clearStr.IndexOf('{', start);
                var indexEnd = clearStr.IndexOf('}', start);
                if (indexEnd >= 0)
                {
                    var length = indexEnd - indexStart + 1;

                    objects.Add(LoadDataObject(str.Substring(indexStart, length)));
                    start = indexEnd + 1;
                }
                else
                    start = -1;
            }

            return objects;
        }

        private DataObject LoadDataObject(string str)
        {
            var clearStr = ReplaceStrings(str);
            var indexStart = clearStr.IndexOf('{') + 1;
            var subStr = new StringBuilder();

            DataObject dataObject = new DataObject();
            Data data = new Data();

            for (int i = indexStart; i < clearStr.Length; i++)
                switch (clearStr[i])
                {
                    case '}':
                    case ';':
                        data.Value = ClearValue(subStr.ToString());
                        if (!string.IsNullOrWhiteSpace(data.Value))
                        {
                            dataObject.Datas.Add(data);
                            data = new Data();
                            subStr.Clear();
                        }

                        break;

                    case ':':
                        data.PropertyName = ClearValue(subStr.ToString());
                        subStr.Clear();
                        break;

                    default:
                        subStr.Append(str[i]);
                        break;
                }

            return dataObject;
        }

        private string ClearValue(string value)
        {
            var res = new StringBuilder(value.Trim(' ').Trim('\t'));
            if (res.Length > 0 && res[0] == '"')
                res.Remove(0, 1);
            if (res.Length > 0 && res[res.Length - 1] == '"')
                res.Remove(res.Length - 1, 1);

            return res.ToString();
        }

        private string ReplaceStrings(string str, char c = 'x')
        {
            var clearStrBuilder = new StringBuilder(str);
            int k = 0;

            for (int i = 0; i < clearStrBuilder.Length; i++)
                switch (clearStrBuilder[i])
                {
                    case '"':
                        if (i > 0 && clearStrBuilder[i - 1] == '\\')
                            clearStrBuilder[i] = c;
                        else
                            k ^= 1;

                        break;

                    default:
                        if (k == 1)
                            clearStrBuilder[i] = c;
                        break;
                }

            return clearStrBuilder.ToString();
        }

        private string GetArrayName(string str)
        {
            int index = str.IndexOf('[');
            int length = index >= 0 ? index : 0;
            return str.Substring(0, length);
        }
    }
}
