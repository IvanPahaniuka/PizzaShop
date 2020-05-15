using Pizza.Plugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizza.Serialization
{
    public sealed class PluginSerializer : ISerializer
    {
        public ISerializer[] Serializers { get; set; }
        public IDataPlugin[] Plugins { get; set; }
        public ISerializer SelectedSerializer { get; set; }
        public IDataPlugin SelectedPlugin { get; set; }

        public PluginSerializer()
            : this(null, null, null, null)
        {

        }
        public PluginSerializer(
            ISerializer selectedSerializer,
            IDataPlugin selectedPlugin)
            : this(null, null, selectedSerializer, selectedPlugin)
        {

        }
        public PluginSerializer(
            IEnumerable<ISerializer> serializers,
            IEnumerable<IDataPlugin> plugins)
            : this(serializers, plugins, null, null)
        {

        }
        public PluginSerializer(
            IEnumerable<ISerializer> serializers,
            IEnumerable<IDataPlugin> plugins,
            ISerializer selectedSerializer,
            IDataPlugin selectedPlugin)
        {
            Serializers = serializers?.ToArray();
            Plugins = plugins?.ToArray();
            SelectedSerializer = selectedSerializer;
            SelectedPlugin = selectedPlugin;
        }


        public T Deserialize<T>(Stream stream) where T : class
        {
            using (var BS = new BinaryReader(stream, Encoding.ASCII))
            {
                int k = 0;
                var stringBuilder = new StringBuilder();
                do
                {
                    var c = BS.ReadChar();
                    stringBuilder.Append(c);
                    if (c == ']')
                        k++;
                }
                while (k < 2);

                var infoStr = stringBuilder.ToString();
                var infos = infoStr.Split(new char[]{'[', ']'}, StringSplitOptions.RemoveEmptyEntries);

                var serializerType = Type.GetType(infos[0], true, true);
                var serializer = Serializers.FirstOrDefault(s => s.GetType().IsEquivalentTo(serializerType));

                var pluginType = infos[1];
                var plugin = Plugins.FirstOrDefault(p => p.GetType().AssemblyQualifiedName == pluginType);

                if (serializer == null || plugin == null)
                    throw new Exception();

                var buffer = BS.BaseStream.ReadToEnd();
                buffer = plugin.Demodify(buffer);
                using (var MS = new MemoryStream(buffer))
                {
                    return serializer.Deserialize<T>(MS);
                }
            }
        }

        public void Serialize<T>(Stream stream, T obj) where T : class
        {
            if (SelectedSerializer == null || SelectedPlugin == null)
                throw new NullReferenceException();

            if (SelectedSerializer is PluginSerializer)
                throw new Exception();

            using (var MS = new MemoryStream())
            {
                SelectedSerializer.Serialize(MS, obj);

                var buffer = MS.GetBuffer();
                buffer = SelectedPlugin.Modify(buffer);

                var serializerName = SelectedSerializer.GetType().AssemblyQualifiedName;
                var pluginName = SelectedPlugin.GetType().AssemblyQualifiedName;
                var infoStr = $"[{serializerName}][{pluginName}]";
                var infoData = Encoding.ASCII.GetBytes(infoStr);

                stream.Write(infoData, 0, infoData.Length);
                stream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
