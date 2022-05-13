using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ObjectManager
{
    public class ObjectSaver : IDisposable
    {
        public ObjectSaver()
        {
            ClearLoadedTypes();
        }

        const byte _Initializer = 0xFF;
        const byte _ByteInit = 0xFF;
        const byte _DataEnd = 0xFE;

        static readonly byte[] DataEnd = { _Initializer, _DataEnd };

        public Header header = new Header();
        /// <summary>
        /// 저장할 경로입니다.
        /// </summary>
        public string path;

        /// <summary>
        /// 저장할 객체입니다.
        /// </summary>
        public object BaseObject;

        /// <summary>
        /// 헤더 추가의 유무입니다.
        /// 추가하지 않으면 불러올때 똑같은 ObjectSaver 객체가 필요합니다.
        /// </summary>
        public bool createHeader = true;

        public class Header
        {
            /// <summary>
            /// 압축방식을 설정합니다.
            /// </summary>
            public CompressionLevel CompressionLevel = CompressionLevel.Fastest;

            /// <summary>
            /// 암호화 여부를 설정합니다. 아직 안만듬
            /// </summary>
            public bool Encrypted = false;

            /// <summary>
            /// true로 설정하면 타입을 쓰거나 읽지 않습니다.
            /// </summary>
            public bool PreLoadedTypes = false;

            /// <summary>
            /// 다른걸로 바뀔예정
            /// </summary>
            public bool MethodInstance = false;

            /// <summary>
            /// 객체의 어셈블리도 같이 저장합니다. 아직 안만듬
            /// </summary>
            public bool FullCopyType = false;
        }

        private List<Type> loadedTypes = new List<Type>(64);

        private void ClearLoadedTypes()
        {
            loadedTypes.Clear();
            loadedTypes.Add(typeof(Int16));
            loadedTypes.Add(typeof(UInt16));
            loadedTypes.Add(typeof(Int32));
            loadedTypes.Add(typeof(UInt32));
            loadedTypes.Add(typeof(Int64));
            loadedTypes.Add(typeof(UInt64));
            loadedTypes.Add(typeof(Single));
            loadedTypes.Add(typeof(Double));
            loadedTypes.Add(typeof(Byte));
            loadedTypes.Add(typeof(SByte));
            loadedTypes.Add(typeof(Decimal));
            loadedTypes.Add(typeof(String));
        }

        private void AssignAllTypes()
        {
            ClearLoadedTypes();
            AssignEverything(BaseObject);
            return;


            void AssignEverything(object obj)
            {
                if (obj == null) return;

                Type t = obj.GetType();

                if (IsPrimitive(t))
                {
                    return;
                }
                if (IsString(t))
                {
                    return;
                }

                AssignType(t);

                if (t.IsArray)
                {
                    AssignArray(obj);
                    return;
                }
                {
                    AssignObject(obj);
                    return;
                }
            }
            void AssignObject(object obj)
            {
                Type baseType = obj.GetType();
                for (int i = 0; baseType.BaseType != null; i++)
                {
                    AssignObjectField(baseType);
                    baseType = baseType.BaseType;
                }


                void AssignObjectField(Type t)
                {
                    BindingFlags bindflags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                    System.Reflection.FieldInfo[] fieldinfo = baseType.GetFields(bindflags);
                    foreach (System.Reflection.FieldInfo info in fieldinfo)
                    {
                        if ((!Attribute.IsDefined(info, typeof(IgnoreSave)) || Attribute.IsDefined(info, typeof(ForceSave))))
                        {
                            AssignEverything(info.GetValue(obj));
                        }
                    }
                }
            }
            void AssignArray(object obj)
            {
                Type t = obj.GetType();
                Type elementType = t.GetElementType();
                if (IsPrimitive(elementType))
                {
                    return;
                }
                if (IsString(elementType))
                {
                    return;
                }
                if (elementType.IsSealed)
                {
                    Array arr = obj as Array;
                    foreach (object o in arr)
                    {
                        AssignEverything(o);
                        if (o != null) return;
                        //sealed라 타입이 다 똑같아서 하나만 보고 끝
                    }
                }
                else
                {
                    Array arr = obj as Array;
                    foreach (object o in arr)
                    {
                        AssignEverything(o);
                    }
                    return;
                }
            }
            void AssignType(Type t)
            {
                if (!loadedTypes.Contains(t))
                {
                    loadedTypes.Add(t);
                }
            }
        }

        private IEnumerable RealLoadedTypes
        {
            get
            {
                return loadedTypes.Skip(12);
            }
        }

        /// <summary>
        /// 경로에 기본 설정을 사용해 객체를 저장합니다.
        /// </summary>
        public static void Save(object obj, string path)
        {
            using (ObjectSaver saver = new ObjectSaver())
            {
                saver.BaseObject = obj;
                saver.path = path;
                saver.Save();
            }
        }

        /// <summary>
        /// 경로에 지정된 설정을 사용해 객체를 저장합니다.
        /// </summary>
        public void Save()
        {
            if(BaseObject == null)
            {
                throw new Exception("오브젝트가 설정되지 않았습니다.");
            }
            if(File.Exists(path))
            {
                File.WriteAllBytes(path, new byte[0]);
            }
            FileStream fs = File.OpenWrite(path);

            GetBytes(fs);

            fs.Close();

            return;
        }

        public byte[] GetBytes()
        {
            MemoryStream ms = new MemoryStream(4096);
            GetBytes(ms);
            byte[] output = ms.ToArray();
            ms.Close();
            return output;
        }

        public void GetBytes(Stream originstream)
        {
            if(!originstream.CanWrite)
            {
                throw new Exception("쓰기 가능한 스트림이 아닙니다.");
            }


            if(this.createHeader)
            {
                originstream.Write(new byte[] { 0x01, GetHeaderByte() });
            }
            else
            {
                originstream.WriteByte(0x00);
            }

            GZipStream stream = new GZipStream(originstream,this.header.CompressionLevel);

            if(!this.header.PreLoadedTypes)
            {
                AssignAllTypes();
            }

            WriteTypeBytes();

            WriteMiddleBytes();

            stream.Close();

            GC.Collect();

            return;

            byte GetHeaderByte()
            {
                byte b = 0x00;
                switch(this.header.CompressionLevel)
                {
                    case CompressionLevel.NoCompression:
                        b++;
                        b++;
                        break;
                    case CompressionLevel.Fastest:
                        b++;
                        break;
                }
                b += (byte)(this.header.Encrypted ? 4 : 0);
                b += (byte)(this.header.PreLoadedTypes ? 8 : 0);
                b += (byte)(this.header.MethodInstance ? 16 : 0);
                b += (byte)(this.header.FullCopyType ? 32 : 0);
                
                

                return b;
            }

            

            void WriteTypeBytes()
            {
                foreach(Type type in this.RealLoadedTypes)
                {
                    StreamWrite(type.FullName.ToByteArray());
                }
                StreamWriteDataEnd();
            }

            void WriteMiddleBytes()
            {
                WriteEverythingBytes(this.BaseObject,"");
                return;


                void WriteEverythingBytes(object obj, string name, bool writeType = true)
                {
                    if (obj == null) return;

                    Type t = obj.GetType();
                    if (writeType)
                    {
                        WriteTypeIndex(t);
                    }

                    if (name != "")
                    {
                        StreamWrite(name.ToByteArray());
                    }


                    if (IsPrimitive(t))
                    {
                        WritePrimitiveBytes(obj);
                        return;
                    }
                    if(IsString(t))
                    {
                        WriteStringBytes(obj);
                        return;
                    }
                    if (IsAwesome(t))
                    {
                        WriteAwesomeBytes(obj);
                        return;
                    }
                    if (t.IsArray)
                    {
                        WriteArrayBytes(obj);
                        return;
                    }
                    else
                    {
                        WriteObjectBytes(obj);
                        return;
                    }
                    
                }

                void WriteObjectBytes(object obj)
                {

                    Type baseType = obj.GetType();
                    for (int i = 0; baseType.BaseType != null; i++)
                    {
                        WriteObjectFieldBytes(obj, baseType, i);
                        baseType = baseType.BaseType;
                    }

                    StreamWriteDataEnd();
                    return;

                    void WriteObjectFieldBytes(object obj, Type baseType, int upper)
                    {
                        string BaseTypeIndex()
                        {
                            char[] output = new char[upper];
                            for (int i = 0; i < upper; i++)
                            {
                                output[i] = '.'; 
                            }
                            return new string(output);
                        }

                        BindingFlags bindflags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                        System.Reflection.FieldInfo[] fieldinfo = baseType.GetFields(bindflags);
                        foreach (System.Reflection.FieldInfo info in fieldinfo)
                        {
                            if (info.GetValue(obj) != null && (!Attribute.IsDefined(info, typeof(IgnoreSave)) || Attribute.IsDefined(info, typeof(ForceSave))))
                            {
                                WriteEverythingBytes(info.GetValue(obj), BaseTypeIndex() + info.Name);
                            }
                        }
                    }
                }

                void WritePrimitiveBytes(object obj, bool dataEnd = false)
                {
                    switch (obj)
                    {
                        case Int16 i:
                            StreamWrite(BitConverter.GetBytes(i).Fix(), dataEnd);
                            return;
                        case UInt16 i:
                            StreamWrite(BitConverter.GetBytes(i).Fix(), dataEnd);
                            return;
                        case Int32 i:
                            StreamWrite(BitConverter.GetBytes(i).Fix(), dataEnd);
                            return;
                        case UInt32 i:
                            StreamWrite(BitConverter.GetBytes(i).Fix(), dataEnd);
                            return;
                        case Int64 i:
                            StreamWrite(BitConverter.GetBytes(i).Fix(), dataEnd);
                            return;
                        case UInt64 i:
                            StreamWrite(BitConverter.GetBytes(i).Fix(), dataEnd);
                            return;
                        case Single i:
                            StreamWrite(BitConverter.GetBytes(i).Fix(), dataEnd);
                            return;
                        case Double i:
                            StreamWrite(BitConverter.GetBytes(i).Fix(), dataEnd);
                            return;
                        case Byte i:
                            StreamWrite(BitConverter.GetBytes(i).Fix(), dataEnd);
                            return;
                        case SByte i:
                            StreamWrite(BitConverter.GetBytes(i).Fix(), dataEnd);
                            return;
                        case Decimal i:
                            StreamWrite(decimal.GetBits(i).Cast<byte>().ToArray().Fix(), dataEnd);
                            return;
                        default:
                            throw new Exception("Primitive가 아님");
                    }
                }
                void WriteStringBytes(object obj)
                {
                    StreamWrite((obj as String).ToByteArray());
                }
                void WriteAwesomeBytes(object obj)
                {
                    throw new NotImplementedException();
                }
                void WriteArrayBytes(object obj)
                {
                    Type t = obj.GetType().GetElementType();

                    if (IsPrimitive(t))//타입명, 크기 생략
                    {
                        WritePrimitiveArrayBytes(obj);
                    }
                    else if(t.IsSealed)//타입명 생략
                    {
                        WriteDynamicArrayBytes(obj, false);
                    }
                    else//다써
                    {
                        WriteDynamicArrayBytes(obj);
                    }

                    void WritePrimitiveArrayBytes(object obj)
                    {
                        Type elementType = obj.GetType().GetElementType();

                        

                        Array objarr = obj as Array;
                        int length = objarr.Length;

                        StreamWrite(length.ToByteArray().Trim());
                        //배열 시작부분 추가 완료

                        foreach (object o in objarr)
                        {
                            WritePrimitiveBytes(o);
                        }

                        objarr = null;
                        //배열 추가 완료
                        return;
                    }
                    void WriteDynamicArrayBytes(object obj, bool writeType = true)
                    {
                        Type elementType = obj.GetType().GetElementType();

                        //배열 시작부분 추가 완료

                        Array objarr = obj as Array;

                        int length = objarr.Length;

                        StreamWrite(length.ToByteArray().Trim());

                        foreach (object o in objarr)
                        {
                            if(o == null)
                            {
                                StreamWrite(new byte[] { 0x00 }, false);
                            }
                            else
                            {
                                StreamWrite(new byte[] { 0x01 }, false);
                            }

                            WriteEverythingBytes(o, "", writeType);
                        }

                        objarr = null;
                        //배열 추가 완료
                        return;
                    }
                }
                
                void WriteTypeIndex(Type t)
                {
                    StreamWrite(BitConverter.GetBytes(loadedTypes.IndexOf(t)).Trim());
                    return;
                }

                
            }


            void StreamWrite(byte[] bytearray, bool dataEnd = true)
            {
                if (dataEnd)
                {
                    stream.Write(bytearray.Fix().Concat(DataEnd).ToArray());
                }
                else
                {
                    stream.Write(bytearray);//고정크기는 Fix 안함
                }
            }
            void StreamWriteDataEnd()
            {
                stream.Write(DataEnd);
            }
        }

        /// <summary>
        /// 경로에 있는 파일에서 기본 설정을 사용해 객체를 불러옵니다.
        /// </summary>
        public static object Load(string path)
        {
            object output;
            using (ObjectSaver saver = new ObjectSaver())
            {
                saver.path = path;
                output = saver.Load();
            }
            return output;
        }

        /// <summary>
        /// 경로에 있는 파일에서 지정된 설정을 사용해 객체를 불러옵니다.
        /// </summary>
        public object Load()
        {
            if(path == null)
            {
                throw new Exception("경로가 설정되지 않았습니다.");
            }
            if(!File.Exists(path))
            {
                throw new Exception("경로에 파일이 없습니다.");
            }

            FileStream fs = File.OpenRead(path);
            GetObject(fs);

            fs.Close();

            return this.BaseObject;
        }

        public object GetObject(byte[] bytearray)
        {
            return GetObject(new MemoryStream(bytearray));
        }

        public object GetObject(Stream originstream)
        {
            if(!originstream.CanRead)
            {
                throw new Exception("읽기 가능한 스트림이 아닙니다.");
            }


            bool useHeader = Convert.ToBoolean(originstream.ReadByte());



            if (useHeader)
            {
                InitHeaderWithByte(originstream.ReadByte());
            }

            GZipStream stream = new GZipStream(originstream, CompressionMode.Decompress);



            if(this.header.PreLoadedTypes)
            {

            }
            else
            {
                ReadLoadedTypes();
            }

            ReadMiddle();

            GC.Collect();

            return this.BaseObject;




            void InitHeaderWithByte(int headerbyte)
            {

                this.header.CompressionLevel = (CompressionLevel)(headerbyte % 4);
                this.header.Encrypted = (headerbyte & 4) == 0 ? false : true;
                this.header.PreLoadedTypes = (headerbyte & 8) == 0 ? false : true;
                this.header.MethodInstance = (headerbyte & 16) == 0 ? false : true;
                this.header.FullCopyType = (headerbyte & 32) == 0 ? false : true;
                
                
            }

            void ReadLoadedTypes()
            {
                byte[] bytearray;
                while (true)
                {
                    bytearray = StreamRead();
                    if (bytearray.Length == 0)
                    {
                        return;
                    }
                    string typename = bytearray.GetString();
                    loadedTypes.Add(GetTypeFixed(typename));
                }
            }

            void ReadMiddle()
            {
                this.BaseObject = GetEverything(false).data;
                return;
                
                (object data, string name) GetEverything(bool readName = true, Type preLoadedType = null)
                {


                    int typeindex;
                    Type type;
                    if (preLoadedType == null)
                    {
                        byte[] imshi = StreamRead();
                        if (imshi.Length == 0)
                        {
                            return (null, "");
                        }
                        typeindex = BitConverter.ToInt32(imshi.Enrich());
                        type = loadedTypes[typeindex];
                    }
                    else
                    {
                        type = preLoadedType;
                        typeindex = loadedTypes.IndexOf(type);
                    }

                    string name = "";

                    if (readName)
                    {
                        byte[] imshi = StreamRead();
                        if (imshi.Length == 0)
                        {
                            return (null, "");
                        }
                        name = imshi.GetString();
                    }
                    
                    object data = null;

                    if (typeindex < 12)//String까지
                    {
                        if (typeindex == 11)//String만 빼고
                        {
                            data =  StreamReadString();
                        }
                        else
                        {
                            data = GetPrimitive(typeindex);
                        }
                    }
                    else if (type.IsArray)
                    {
                        GetArray(type);
                    }
                    else if (IsAwesome(type))
                    {
                        data = GetAwesome(type);
                    }
                    else if (IsIteratorBlock(type))
                    {
                        data = GetObject(type, Activator.CreateInstance(type, 0));
                    }
                    else//IsObject
                    {
                        data = GetObject(type);
                    }
                    return (data, name);
                }
                object GetObject(Type t, object baseObj = null)
                {
                    
                    object o = baseObj == null ? Activator.CreateInstance(t) : baseObj;
                    List<Type> baseTypes = GetBaseTypes(t);
                    List<FieldInfo[]> fieldInfos = GetBaseFieldInfos(baseTypes);

                    BindingFlags bindflags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                    System.Reflection.FieldInfo[] fieldinfos = t.GetFields(bindflags);
                    while (true)
                    {
                        (object fieldobj, string fieldname) = GetEverything();
                        if(fieldobj == null)
                        {
                            break;
                        }
                        SetFieldFixed(fieldobj, fieldname);
                    }
                    return o;

                    List<Type> GetBaseTypes(Type baseType)
                    {
                        List<Type> baseTypeList = new List<Type>(8);
                        baseTypeList.Add(baseType);
                        Type t = baseType;
                        while (t.BaseType != null)
                        {
                            baseTypeList.Add(t.BaseType);
                            t = t.BaseType;
                        }

                        return baseTypeList;
                    }
                    List<FieldInfo[]> GetBaseFieldInfos(List<Type> types)
                    {
                        BindingFlags bindflags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                        List<FieldInfo[]> fieldinfos = new List<FieldInfo[]>(128);
                        foreach (Type type in types)
                        {
                            fieldinfos.Add(type.GetFields(bindflags));
                        }
                        return fieldinfos;
                    }
                    void SetFieldFixed(object fieldobj, string fieldname)
                    {
                        int i = 0;
                        while(true)
                        {
                            if(fieldname[0] == '.')
                            {
                                    i++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        string realfieldname = new string(fieldname.Skip(i).ToArray());
                        FieldInfo[] realInfos = fieldInfos[i];
                        foreach (FieldInfo info in realInfos)
                        {
                            if (info.Name == fieldname)
                            {
                                info.SetValue(o, fieldobj);
                                return;
                            }
                        }
                        throw new Exception("값이 설정되지 못했습니다.");
                    }
                }
                Array GetArray(Type t)
                {
                    Type elementType = t.GetElementType();
                    if (IsPrimitive(elementType))//타입명, 크기 생략
                    {
                        return GetPrimitiveArray(t);
                    }
                    else if (t.IsSealed)//타입명 생략
                    {
                        return GetDynamicArray(t, false);
                    }
                    else//다써
                    {
                        return GetDynamicArray(t);
                    }

                    Array GetPrimitiveArray(Type t)
                    {
                        int length = BitConverter.ToInt32(StreamRead().Enrich());//배열 갯수
                        Type elementtype = t.GetElementType();
                        Array array = Array.CreateInstance(elementtype,length);
                        int elementtypeindex = loadedTypes.IndexOf(elementtype);
                        for(int i=0;i<length;i++)
                        {
                            array.SetValue(GetPrimitive(elementtypeindex), i);
                        }

                        return array;
                    }
                    Array GetDynamicArray(Type t, bool readElementType = true)
                    {
                        int length = BitConverter.ToInt32(StreamRead().Enrich());//배열 갯수
                        Type elementtype = t.GetElementType();
                        Array array = Array.CreateInstance(elementtype, length);

                        Type preLoadedType = readElementType ? null : elementtype;

                        byte nullcheck;
                        for (int i=0;i<length;i++)
                        {
                            nullcheck = StreamReadSized(1)[0];
                            if(nullcheck == 0x01)
                            {
                                array.SetValue(GetEverything(false, preLoadedType).data, i);
                            }
                        }
                        return array;
                    }
                }

                object GetPrimitive(int typeindex)
                {
                    object imshi;
                    switch (typeindex)
                    {
                        case 0:
                            imshi = BitConverter.ToInt16(StreamReadSized(2));
                            break;
                        case 1:
                            imshi = BitConverter.ToUInt16(StreamReadSized(2));
                            break;
                        case 2:
                            imshi = BitConverter.ToInt32(StreamReadSized(4));
                            break;
                        case 3:
                            imshi = BitConverter.ToUInt32(StreamReadSized(4));
                            break;
                        case 4:
                            imshi = BitConverter.ToInt64(StreamReadSized(8));
                            break;
                        case 5:
                            imshi = BitConverter.ToUInt64(StreamReadSized(8));
                            break;
                        case 6:
                            imshi = BitConverter.ToSingle(StreamReadSized(4));
                            break;
                        case 7:
                            imshi = BitConverter.ToDouble(StreamReadSized(8));
                            break;
                        case 8:
                            throw new NotImplementedException();
                        case 9:
                            throw new NotImplementedException();
                        case 10:
                            imshi = ByteArrayToDecimal(StreamReadSized(16));
                            break;
                        default:
                            throw new Exception("오류");



                    }
                    return imshi;


                    const byte DecimalSignBit = 128;
                    decimal ByteArrayToDecimal(byte[] src, int offset = 0)
                    {
                        return new decimal(
                            BitConverter.ToInt32(src, offset),
                            BitConverter.ToInt32(src, offset + 4),
                            BitConverter.ToInt32(src, offset + 8),
                            src[offset + 15] == DecimalSignBit,
                            src[offset + 14]);
                    }
                }
                object GetAwesome(Type t)
                {
                    object data;
                    using (var ms = new MemoryStream(StreamRead()))
                    {
                        data = new BinaryFormatter().Deserialize(ms);
                    }
                    return data;
                }
            }

            
            byte[] StreamRead()
            {
                List<byte> bytelist = new List<byte>(32);
                byte b;
                byte b2;
                while(true)
                {
                    b = Convert.ToByte(stream.ReadByte());
                    if(b == _Initializer)
                    {
                        b2 = Convert.ToByte(stream.ReadByte());
                        switch(b2)
                        {
                            case _DataEnd:
                                if(bytelist.Count == 0)
                                {
                                    return new byte[0];
                                }
                                return bytelist.ToArray();
                            default:
                                bytelist.Add(_Initializer);
                                bytelist.Add(b2);
                                break;
                        }
                    }
                    else
                    {
                        bytelist.Add(b);
                    }

                }
            }
            byte[] StreamReadSized(int count)
            {
                byte[] bytearray = new byte[count];
                stream.Read(bytearray, 0, count);
                return bytearray;
            }
            string StreamReadString()
            {
                return Encoding.UTF8.GetString(StreamRead());
            }
            
        }


        bool IsPrimitive(Type t)
        {
            if (t.IsPrimitive || t == typeof(Decimal))
            {
                return true;
            }
            return false;
        }
        bool IsString(Type t)
        {
            if(t == typeof(String))
            {
                return true;
            }
            return false;
        }
        bool IsAwesome(Type t)
        {
            if (IsCapsuledMethod(t))
            {
                return true;
            }
            return false;
        }
        bool IsIteratorBlock(Type t)
        {
            if (
                Attribute.IsDefined(t, typeof(CompilerGeneratedAttribute))
                &&
                t.IsSealed
                &&
                t.GetInterfaces().Contains(typeof(IEnumerator))
            )
            {
                return true;
            }

            return false;
        }
        bool IsCapsuledMethod(Type t)
        {
            if (
                IsAction(t)
                ||
                IsFunc(t)
            )
            {
                return true;
            }
            return false;
        }
        bool IsAction(Type t)
        {
            if (t == typeof(System.Action)) return true;
            Type generic = null;
            if (t.IsGenericTypeDefinition) generic = t;
            else if (t.IsGenericType) generic = t.GetGenericTypeDefinition();

            if (generic == null) return false;
            if (generic == typeof(System.Action<>)) return true;
            if (generic == typeof(System.Action<,>)) return true;
            if (generic == typeof(System.Action<,,>)) return true;
            if (generic == typeof(System.Action<,,,>)) return true;
            if (generic == typeof(System.Action<,,,,>)) return true;
            if (generic == typeof(System.Action<,,,,,>)) return true;
            if (generic == typeof(System.Action<,,,,,,>)) return true;
            if (generic == typeof(System.Action<,,,,,,,>)) return true;
            if (generic == typeof(System.Action<,,,,,,,,>)) return true;
            if (generic == typeof(System.Action<,,,,,,,,,>)) return true;
            if (generic == typeof(System.Action<,,,,,,,,,,>)) return true;
            if (generic == typeof(System.Action<,,,,,,,,,,,>)) return true;
            if (generic == typeof(System.Action<,,,,,,,,,,,,>)) return true;
            if (generic == typeof(System.Action<,,,,,,,,,,,,,>)) return true;
            if (generic == typeof(System.Action<,,,,,,,,,,,,,,>)) return true;
            if (generic == typeof(System.Action<,,,,,,,,,,,,,,,>)) return true;

            return false;
        }
        bool IsFunc(Type t)
        {
            Type generic = null;
            if (t.IsGenericTypeDefinition) generic = t;
            else if (t.IsGenericType) generic = t.GetGenericTypeDefinition();

            if (generic == null) return false;
            if (generic == typeof(System.Func<>)) return true;
            if (generic == typeof(System.Func<,>)) return true;
            if (generic == typeof(System.Func<,,>)) return true;
            if (generic == typeof(System.Func<,,,>)) return true;
            if (generic == typeof(System.Func<,,,,>)) return true;
            if (generic == typeof(System.Func<,,,,,>)) return true;
            if (generic == typeof(System.Func<,,,,,,>)) return true;
            if (generic == typeof(System.Func<,,,,,,,>)) return true;
            if (generic == typeof(System.Func<,,,,,,,,>)) return true;
            if (generic == typeof(System.Func<,,,,,,,,,>)) return true;
            if (generic == typeof(System.Func<,,,,,,,,,,>)) return true;
            if (generic == typeof(System.Func<,,,,,,,,,,,>)) return true;
            if (generic == typeof(System.Func<,,,,,,,,,,,,>)) return true;
            if (generic == typeof(System.Func<,,,,,,,,,,,,,>)) return true;
            if (generic == typeof(System.Func<,,,,,,,,,,,,,,>)) return true;
            if (generic == typeof(System.Func<,,,,,,,,,,,,,,,>)) return true;
            if (generic == typeof(System.Func<,,,,,,,,,,,,,,,,>)) return true;

            return false;
        }

        Type GetTypeFixed(string str)
        {
            Type t = Type.GetType(str);
            if (t != null)
            {
                return t;
            }
            else
            {
                foreach (Assembly assembly in LoadedAssemblies)
                {
                    t = assembly.GetType(str);
                    if (t != null)
                        return t;
                }
            }
            return null;
        }

        public static Assembly[] LoadedAssemblies;

        static ObjectSaver()
        {
            LoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.path = null;
                    this.header = null;
                    this.loadedTypes.Clear();
                    this.loadedTypes = null;
                    
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.

                }

                this.BaseObject = null;

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~ObjectSaver()
        // {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        void IDisposable.Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion


    }
    internal static class ExtensionClass
    {
        const byte _Initializer = 0xFF;
        const byte _ByteInit = 0xFF;
        const byte _DataEnd = 0xFE;

        internal static byte[] Enrich(this byte[] bytearray)
        {
            byte[] output = new byte[] { 0x00, 0x00, 0x00, 0x00 };
            if(bytearray.Length > 4)
            {
                throw new Exception();
            }
            for(int i=0;i<bytearray.Length;i++)
            {
                output[i] = bytearray[i];
            }

            return output;
        }
        internal static byte[] ToByteArray(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        internal static byte[] ToByteArray(this int i)
        {
            return BitConverter.GetBytes(i).Trim();
        }

        internal static string GetString(this byte[] bytearray)
        {
            return Encoding.UTF8.GetString(bytearray);
        }
        internal static string GetString(this List<byte> bytearray)
        {
            return Encoding.UTF8.GetString(bytearray.ToArray());
        }

        internal static byte[] Fix(this byte[] bytearray)
        {
            List<byte> newlist = new List<byte>(bytearray.Length * 2);
            foreach(byte b in bytearray)
            {
                if(b == _Initializer)
                {
                    newlist.Add(_Initializer);
                    newlist.Add(_ByteInit);
                }
                else
                {
                    newlist.Add(b);
                }
            }
            return newlist.ToArray();
        }

        internal static byte[] Trim(this byte[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == 0x00)
                {
                    return input.Take(i).ToArray();
                }
            }
            return input;
        }
    }


    public class IgnoreSave : Attribute
    {

    }
    public class ForceSave : Attribute
    {

    }
    public class SaveTypeItself : Attribute
    {

    }
    public class SaveMethodAsInstance : Attribute
    {

    }
}
