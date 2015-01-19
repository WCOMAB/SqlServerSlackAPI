//Disable documentation warning for external code
#pragma warning disable 1591
#pragma warning disable 1587
// ############################################################################
// #                                                                          #
// #        ---==>  T H I S  F I L E  I S   G E N E R A T E D  <==---         #
// #                                                                          #
// # This means that any edits to the .cs file will be lost when its          #
// # regenerated. Changes should instead be applied to the corresponding      #
// # text template file (.tt)                                                 #
// ############################################################################



// ############################################################################
// @@@ INCLUDING: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/IJsonWrapper.cs
// @@@ INCLUDING: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonData.cs
// @@@ INCLUDING: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonException.cs
// @@@ INCLUDING: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonMapper.cs
// @@@ INCLUDING: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonReader.cs?
// @@@ INCLUDING: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonWriter.cs
// @@@ INCLUDING: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonMockWrapper.cs
// @@@ INCLUDING: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/Lexer.cs
// @@@ INCLUDING: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/ParserToken.cs
// ############################################################################
// Certains directives such as #define and // Resharper comments has to be 
// moved to top in order to work properly    
// ############################################################################
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/IJsonWrapper.cs
namespace SqlServerSlackAPI
{
    #region Header
    /**
     * IJsonWrapper.cs
     *   Interface that represents a type capable of handling all kinds of JSON
     *   data. This is mainly used when mapping objects through JsonMapper, and
     *   it's implemented by JsonData.
     *
     * The authors disclaim copyright to this source code. For more details, see
     * the COPYING file included with this distribution.
     **/
    #endregion
    
    
    using System.Collections;
    using System.Collections.Specialized;
    
    
    namespace LitJson
    {
        public enum JsonType
        {
            None,
    
            Object,
            Array,
            String,
            Int,
            Long,
            Double,
            Boolean
        }
    
        public interface IJsonWrapper : IList, IOrderedDictionary
        {
            bool IsArray   { get; }
            bool IsBoolean { get; }
            bool IsDouble  { get; }
            bool IsInt     { get; }
            bool IsLong    { get; }
            bool IsObject  { get; }
            bool IsString  { get; }
    
            bool     GetBoolean ();
            double   GetDouble ();
            int      GetInt ();
            JsonType GetJsonType ();
            long     GetLong ();
            string   GetString ();
    
            void SetBoolean  (bool val);
            void SetDouble   (double val);
            void SetInt      (int val);
            void SetJsonType (JsonType type);
            void SetLong     (long val);
            void SetString   (string val);
    
            string ToJson ();
            void   ToJson (JsonWriter writer);
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/IJsonWrapper.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonData.cs
namespace SqlServerSlackAPI
{
    #region Header
    /**
     * JsonData.cs
     *   Generic type to hold JSON data (objects, arrays, and so on). This is
     *   the default type returned by JsonMapper.ToObject().
     *
     * The authors disclaim copyright to this source code. For more details, see
     * the COPYING file included with this distribution.
     **/
    #endregion
    
    
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    
    
    namespace LitJson
    {
        public class JsonData : IJsonWrapper, IEquatable<JsonData>
        {
            #region Fields
            private IList<JsonData>               inst_array;
            private bool                          inst_boolean;
            private double                        inst_double;
            private int                           inst_int;
            private long                          inst_long;
            private IDictionary<string, JsonData> inst_object;
            private string                        inst_string;
            private string                        json;
            private JsonType                      type;
    
            // Used to implement the IOrderedDictionary interface
            private IList<KeyValuePair<string, JsonData>> object_list;
            #endregion
    
    
            #region Properties
            public int Count {
                get { return EnsureCollection ().Count; }
            }
    
            public bool IsArray {
                get { return type == JsonType.Array; }
            }
    
            public bool IsBoolean {
                get { return type == JsonType.Boolean; }
            }
    
            public bool IsDouble {
                get { return type == JsonType.Double; }
            }
    
            public bool IsInt {
                get { return type == JsonType.Int; }
            }
    
            public bool IsLong {
                get { return type == JsonType.Long; }
            }
    
            public bool IsObject {
                get { return type == JsonType.Object; }
            }
    
            public bool IsString {
                get { return type == JsonType.String; }
            }
    
            public ICollection<string> Keys {
                get { EnsureDictionary (); return inst_object.Keys; }
            }
            #endregion
    
    
            #region ICollection Properties
            int ICollection.Count {
                get {
                    return Count;
                }
            }
    
            bool ICollection.IsSynchronized {
                get {
                    return EnsureCollection ().IsSynchronized;
                }
            }
    
            object ICollection.SyncRoot {
                get {
                    return EnsureCollection ().SyncRoot;
                }
            }
            #endregion
    
    
            #region IDictionary Properties
            bool IDictionary.IsFixedSize {
                get {
                    return EnsureDictionary ().IsFixedSize;
                }
            }
    
            bool IDictionary.IsReadOnly {
                get {
                    return EnsureDictionary ().IsReadOnly;
                }
            }
    
            ICollection IDictionary.Keys {
                get {
                    EnsureDictionary ();
                    IList<string> keys = new List<string> ();
    
                    foreach (KeyValuePair<string, JsonData> entry in
                             object_list) {
                        keys.Add (entry.Key);
                    }
    
                    return (ICollection) keys;
                }
            }
    
            ICollection IDictionary.Values {
                get {
                    EnsureDictionary ();
                    IList<JsonData> values = new List<JsonData> ();
    
                    foreach (KeyValuePair<string, JsonData> entry in
                             object_list) {
                        values.Add (entry.Value);
                    }
    
                    return (ICollection) values;
                }
            }
            #endregion
    
    
    
            #region IJsonWrapper Properties
            bool IJsonWrapper.IsArray {
                get { return IsArray; }
            }
    
            bool IJsonWrapper.IsBoolean {
                get { return IsBoolean; }
            }
    
            bool IJsonWrapper.IsDouble {
                get { return IsDouble; }
            }
    
            bool IJsonWrapper.IsInt {
                get { return IsInt; }
            }
    
            bool IJsonWrapper.IsLong {
                get { return IsLong; }
            }
    
            bool IJsonWrapper.IsObject {
                get { return IsObject; }
            }
    
            bool IJsonWrapper.IsString {
                get { return IsString; }
            }
            #endregion
    
    
            #region IList Properties
            bool IList.IsFixedSize {
                get {
                    return EnsureList ().IsFixedSize;
                }
            }
    
            bool IList.IsReadOnly {
                get {
                    return EnsureList ().IsReadOnly;
                }
            }
            #endregion
    
    
            #region IDictionary Indexer
            object IDictionary.this[object key] {
                get {
                    return EnsureDictionary ()[key];
                }
    
                set {
                    if (! (key is String))
                        throw new ArgumentException (
                            "The key has to be a string");
    
                    JsonData data = ToJsonData (value);
    
                    this[(string) key] = data;
                }
            }
            #endregion
    
    
            #region IOrderedDictionary Indexer
            object IOrderedDictionary.this[int idx] {
                get {
                    EnsureDictionary ();
                    return object_list[idx].Value;
                }
    
                set {
                    EnsureDictionary ();
                    JsonData data = ToJsonData (value);
    
                    KeyValuePair<string, JsonData> old_entry = object_list[idx];
    
                    inst_object[old_entry.Key] = data;
    
                    KeyValuePair<string, JsonData> entry =
                        new KeyValuePair<string, JsonData> (old_entry.Key, data);
    
                    object_list[idx] = entry;
                }
            }
            #endregion
    
    
            #region IList Indexer
            object IList.this[int index] {
                get {
                    return EnsureList ()[index];
                }
    
                set {
                    EnsureList ();
                    JsonData data = ToJsonData (value);
    
                    this[index] = data;
                }
            }
            #endregion
    
    
            #region Public Indexers
            public JsonData this[string prop_name] {
                get {
                    EnsureDictionary ();
                    return inst_object[prop_name];
                }
    
                set {
                    EnsureDictionary ();
    
                    KeyValuePair<string, JsonData> entry =
                        new KeyValuePair<string, JsonData> (prop_name, value);
    
                    if (inst_object.ContainsKey (prop_name)) {
                        for (int i = 0; i < object_list.Count; i++) {
                            if (object_list[i].Key == prop_name) {
                                object_list[i] = entry;
                                break;
                            }
                        }
                    } else
                        object_list.Add (entry);
    
                    inst_object[prop_name] = value;
    
                    json = null;
                }
            }
    
            public JsonData this[int index] {
                get {
                    EnsureCollection ();
    
                    if (type == JsonType.Array)
                        return inst_array[index];
    
                    return object_list[index].Value;
                }
    
                set {
                    EnsureCollection ();
    
                    if (type == JsonType.Array)
                        inst_array[index] = value;
                    else {
                        KeyValuePair<string, JsonData> entry = object_list[index];
                        KeyValuePair<string, JsonData> new_entry =
                            new KeyValuePair<string, JsonData> (entry.Key, value);
    
                        object_list[index] = new_entry;
                        inst_object[entry.Key] = value;
                    }
    
                    json = null;
                }
            }
            #endregion
    
    
            #region Constructors
            public JsonData ()
            {
            }
    
            public JsonData (bool boolean)
            {
                type = JsonType.Boolean;
                inst_boolean = boolean;
            }
    
            public JsonData (double number)
            {
                type = JsonType.Double;
                inst_double = number;
            }
    
            public JsonData (int number)
            {
                type = JsonType.Int;
                inst_int = number;
            }
    
            public JsonData (long number)
            {
                type = JsonType.Long;
                inst_long = number;
            }
    
            public JsonData (object obj)
            {
                if (obj is Boolean) {
                    type = JsonType.Boolean;
                    inst_boolean = (bool) obj;
                    return;
                }
    
                if (obj is Double) {
                    type = JsonType.Double;
                    inst_double = (double) obj;
                    return;
                }
    
                if (obj is Int32) {
                    type = JsonType.Int;
                    inst_int = (int) obj;
                    return;
                }
    
                if (obj is Int64) {
                    type = JsonType.Long;
                    inst_long = (long) obj;
                    return;
                }
    
                if (obj is String) {
                    type = JsonType.String;
                    inst_string = (string) obj;
                    return;
                }
    
                throw new ArgumentException (
                    "Unable to wrap the given object with JsonData");
            }
    
            public JsonData (string str)
            {
                type = JsonType.String;
                inst_string = str;
            }
            #endregion
    
    
            #region Implicit Conversions
            public static implicit operator JsonData (Boolean data)
            {
                return new JsonData (data);
            }
    
            public static implicit operator JsonData (Double data)
            {
                return new JsonData (data);
            }
    
            public static implicit operator JsonData (Int32 data)
            {
                return new JsonData (data);
            }
    
            public static implicit operator JsonData (Int64 data)
            {
                return new JsonData (data);
            }
    
            public static implicit operator JsonData (String data)
            {
                return new JsonData (data);
            }
            #endregion
    
    
            #region Explicit Conversions
            public static explicit operator Boolean (JsonData data)
            {
                if (data.type != JsonType.Boolean)
                    throw new InvalidCastException (
                        "Instance of JsonData doesn't hold a double");
    
                return data.inst_boolean;
            }
    
            public static explicit operator Double (JsonData data)
            {
                if (data.type != JsonType.Double)
                    throw new InvalidCastException (
                        "Instance of JsonData doesn't hold a double");
    
                return data.inst_double;
            }
    
            public static explicit operator Int32 (JsonData data)
            {
                if (data.type != JsonType.Int)
                    throw new InvalidCastException (
                        "Instance of JsonData doesn't hold an int");
    
                return data.inst_int;
            }
    
            public static explicit operator Int64 (JsonData data)
            {
                if (data.type != JsonType.Long)
                    throw new InvalidCastException (
                        "Instance of JsonData doesn't hold an int");
    
                return data.inst_long;
            }
    
            public static explicit operator String (JsonData data)
            {
                if (data.type != JsonType.String)
                    throw new InvalidCastException (
                        "Instance of JsonData doesn't hold a string");
    
                return data.inst_string;
            }
            #endregion
    
    
            #region ICollection Methods
            void ICollection.CopyTo (Array array, int index)
            {
                EnsureCollection ().CopyTo (array, index);
            }
            #endregion
    
    
            #region IDictionary Methods
            void IDictionary.Add (object key, object value)
            {
                JsonData data = ToJsonData (value);
    
                EnsureDictionary ().Add (key, data);
    
                KeyValuePair<string, JsonData> entry =
                    new KeyValuePair<string, JsonData> ((string) key, data);
                object_list.Add (entry);
    
                json = null;
            }
    
            void IDictionary.Clear ()
            {
                EnsureDictionary ().Clear ();
                object_list.Clear ();
                json = null;
            }
    
            bool IDictionary.Contains (object key)
            {
                return EnsureDictionary ().Contains (key);
            }
    
            IDictionaryEnumerator IDictionary.GetEnumerator ()
            {
                return ((IOrderedDictionary) this).GetEnumerator ();
            }
    
            void IDictionary.Remove (object key)
            {
                EnsureDictionary ().Remove (key);
    
                for (int i = 0; i < object_list.Count; i++) {
                    if (object_list[i].Key == (string) key) {
                        object_list.RemoveAt (i);
                        break;
                    }
                }
    
                json = null;
            }
            #endregion
    
    
            #region IEnumerable Methods
            IEnumerator IEnumerable.GetEnumerator ()
            {
                return EnsureCollection ().GetEnumerator ();
            }
            #endregion
    
    
            #region IJsonWrapper Methods
            bool IJsonWrapper.GetBoolean ()
            {
                if (type != JsonType.Boolean)
                    throw new InvalidOperationException (
                        "JsonData instance doesn't hold a boolean");
    
                return inst_boolean;
            }
    
            double IJsonWrapper.GetDouble ()
            {
                if (type != JsonType.Double)
                    throw new InvalidOperationException (
                        "JsonData instance doesn't hold a double");
    
                return inst_double;
            }
    
            int IJsonWrapper.GetInt ()
            {
                if (type != JsonType.Int)
                    throw new InvalidOperationException (
                        "JsonData instance doesn't hold an int");
    
                return inst_int;
            }
    
            long IJsonWrapper.GetLong ()
            {
                if (type != JsonType.Long)
                    throw new InvalidOperationException (
                        "JsonData instance doesn't hold a long");
    
                return inst_long;
            }
    
            string IJsonWrapper.GetString ()
            {
                if (type != JsonType.String)
                    throw new InvalidOperationException (
                        "JsonData instance doesn't hold a string");
    
                return inst_string;
            }
    
            void IJsonWrapper.SetBoolean (bool val)
            {
                type = JsonType.Boolean;
                inst_boolean = val;
                json = null;
            }
    
            void IJsonWrapper.SetDouble (double val)
            {
                type = JsonType.Double;
                inst_double = val;
                json = null;
            }
    
            void IJsonWrapper.SetInt (int val)
            {
                type = JsonType.Int;
                inst_int = val;
                json = null;
            }
    
            void IJsonWrapper.SetLong (long val)
            {
                type = JsonType.Long;
                inst_long = val;
                json = null;
            }
    
            void IJsonWrapper.SetString (string val)
            {
                type = JsonType.String;
                inst_string = val;
                json = null;
            }
    
            string IJsonWrapper.ToJson ()
            {
                return ToJson ();
            }
    
            void IJsonWrapper.ToJson (JsonWriter writer)
            {
                ToJson (writer);
            }
            #endregion
    
    
            #region IList Methods
            int IList.Add (object value)
            {
                return Add (value);
            }
    
            void IList.Clear ()
            {
                EnsureList ().Clear ();
                json = null;
            }
    
            bool IList.Contains (object value)
            {
                return EnsureList ().Contains (value);
            }
    
            int IList.IndexOf (object value)
            {
                return EnsureList ().IndexOf (value);
            }
    
            void IList.Insert (int index, object value)
            {
                EnsureList ().Insert (index, value);
                json = null;
            }
    
            void IList.Remove (object value)
            {
                EnsureList ().Remove (value);
                json = null;
            }
    
            void IList.RemoveAt (int index)
            {
                EnsureList ().RemoveAt (index);
                json = null;
            }
            #endregion
    
    
            #region IOrderedDictionary Methods
            IDictionaryEnumerator IOrderedDictionary.GetEnumerator ()
            {
                EnsureDictionary ();
    
                return new OrderedDictionaryEnumerator (
                    object_list.GetEnumerator ());
            }
    
            void IOrderedDictionary.Insert (int idx, object key, object value)
            {
                string property = (string) key;
                JsonData data  = ToJsonData (value);
    
                this[property] = data;
    
                KeyValuePair<string, JsonData> entry =
                    new KeyValuePair<string, JsonData> (property, data);
    
                object_list.Insert (idx, entry);
            }
    
            void IOrderedDictionary.RemoveAt (int idx)
            {
                EnsureDictionary ();
    
                inst_object.Remove (object_list[idx].Key);
                object_list.RemoveAt (idx);
            }
            #endregion
    
    
            #region Private Methods
            private ICollection EnsureCollection ()
            {
                if (type == JsonType.Array)
                    return (ICollection) inst_array;
    
                if (type == JsonType.Object)
                    return (ICollection) inst_object;
    
                throw new InvalidOperationException (
                    "The JsonData instance has to be initialized first");
            }
    
            private IDictionary EnsureDictionary ()
            {
                if (type == JsonType.Object)
                    return (IDictionary) inst_object;
    
                if (type != JsonType.None)
                    throw new InvalidOperationException (
                        "Instance of JsonData is not a dictionary");
    
                type = JsonType.Object;
                inst_object = new Dictionary<string, JsonData> ();
                object_list = new List<KeyValuePair<string, JsonData>> ();
    
                return (IDictionary) inst_object;
            }
    
            private IList EnsureList ()
            {
                if (type == JsonType.Array)
                    return (IList) inst_array;
    
                if (type != JsonType.None)
                    throw new InvalidOperationException (
                        "Instance of JsonData is not a list");
    
                type = JsonType.Array;
                inst_array = new List<JsonData> ();
    
                return (IList) inst_array;
            }
    
            private JsonData ToJsonData (object obj)
            {
                if (obj == null)
                    return null;
    
                if (obj is JsonData)
                    return (JsonData) obj;
    
                return new JsonData (obj);
            }
    
            private static void WriteJson (IJsonWrapper obj, JsonWriter writer)
            {
                if (obj == null) {
                    writer.Write (null);
                    return;
                }
    
                if (obj.IsString) {
                    writer.Write (obj.GetString ());
                    return;
                }
    
                if (obj.IsBoolean) {
                    writer.Write (obj.GetBoolean ());
                    return;
                }
    
                if (obj.IsDouble) {
                    writer.Write (obj.GetDouble ());
                    return;
                }
    
                if (obj.IsInt) {
                    writer.Write (obj.GetInt ());
                    return;
                }
    
                if (obj.IsLong) {
                    writer.Write (obj.GetLong ());
                    return;
                }
    
                if (obj.IsArray) {
                    writer.WriteArrayStart ();
                    foreach (object elem in (IList) obj)
                        WriteJson ((JsonData) elem, writer);
                    writer.WriteArrayEnd ();
    
                    return;
                }
    
                if (obj.IsObject) {
                    writer.WriteObjectStart ();
    
                    foreach (DictionaryEntry entry in ((IDictionary) obj)) {
                        writer.WritePropertyName ((string) entry.Key);
                        WriteJson ((JsonData) entry.Value, writer);
                    }
                    writer.WriteObjectEnd ();
    
                    return;
                }
            }
            #endregion
    
    
            public int Add (object value)
            {
                JsonData data = ToJsonData (value);
    
                json = null;
    
                return EnsureList ().Add (data);
            }
    
            public void Clear ()
            {
                if (IsObject) {
                    ((IDictionary) this).Clear ();
                    return;
                }
    
                if (IsArray) {
                    ((IList) this).Clear ();
                    return;
                }
            }
    
            public bool Equals (JsonData x)
            {
                if (x == null)
                    return false;
    
                if (x.type != this.type)
                    return false;
    
                switch (this.type) {
                case JsonType.None:
                    return true;
    
                case JsonType.Object:
                    return this.inst_object.Equals (x.inst_object);
    
                case JsonType.Array:
                    return this.inst_array.Equals (x.inst_array);
    
                case JsonType.String:
                    return this.inst_string.Equals (x.inst_string);
    
                case JsonType.Int:
                    return this.inst_int.Equals (x.inst_int);
    
                case JsonType.Long:
                    return this.inst_long.Equals (x.inst_long);
    
                case JsonType.Double:
                    return this.inst_double.Equals (x.inst_double);
    
                case JsonType.Boolean:
                    return this.inst_boolean.Equals (x.inst_boolean);
                }
    
                return false;
            }
    
            public JsonType GetJsonType ()
            {
                return type;
            }
    
            public void SetJsonType (JsonType type)
            {
                if (this.type == type)
                    return;
    
                switch (type) {
                case JsonType.None:
                    break;
    
                case JsonType.Object:
                    inst_object = new Dictionary<string, JsonData> ();
                    object_list = new List<KeyValuePair<string, JsonData>> ();
                    break;
    
                case JsonType.Array:
                    inst_array = new List<JsonData> ();
                    break;
    
                case JsonType.String:
                    inst_string = default (String);
                    break;
    
                case JsonType.Int:
                    inst_int = default (Int32);
                    break;
    
                case JsonType.Long:
                    inst_long = default (Int64);
                    break;
    
                case JsonType.Double:
                    inst_double = default (Double);
                    break;
    
                case JsonType.Boolean:
                    inst_boolean = default (Boolean);
                    break;
                }
    
                this.type = type;
            }
    
            public string ToJson ()
            {
                if (json != null)
                    return json;
    
                StringWriter sw = new StringWriter ();
                JsonWriter writer = new JsonWriter (sw);
                writer.Validate = false;
    
                WriteJson (this, writer);
                json = sw.ToString ();
    
                return json;
            }
    
            public void ToJson (JsonWriter writer)
            {
                bool old_validate = writer.Validate;
    
                writer.Validate = false;
    
                WriteJson (this, writer);
    
                writer.Validate = old_validate;
            }
    
            public override string ToString ()
            {
                switch (type) {
                case JsonType.Array:
                    return "JsonData array";
    
                case JsonType.Boolean:
                    return inst_boolean.ToString ();
    
                case JsonType.Double:
                    return inst_double.ToString ();
    
                case JsonType.Int:
                    return inst_int.ToString ();
    
                case JsonType.Long:
                    return inst_long.ToString ();
    
                case JsonType.Object:
                    return "JsonData object";
    
                case JsonType.String:
                    return inst_string;
                }
    
                return "Uninitialized JsonData";
            }
        }
    
    
        internal class OrderedDictionaryEnumerator : IDictionaryEnumerator
        {
            IEnumerator<KeyValuePair<string, JsonData>> list_enumerator;
    
    
            public object Current {
                get { return Entry; }
            }
    
            public DictionaryEntry Entry {
                get {
                    KeyValuePair<string, JsonData> curr = list_enumerator.Current;
                    return new DictionaryEntry (curr.Key, curr.Value);
                }
            }
    
            public object Key {
                get { return list_enumerator.Current.Key; }
            }
    
            public object Value {
                get { return list_enumerator.Current.Value; }
            }
    
    
            public OrderedDictionaryEnumerator (
                IEnumerator<KeyValuePair<string, JsonData>> enumerator)
            {
                list_enumerator = enumerator;
            }
    
    
            public bool MoveNext ()
            {
                return list_enumerator.MoveNext ();
            }
    
            public void Reset ()
            {
                list_enumerator.Reset ();
            }
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonData.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonException.cs
namespace SqlServerSlackAPI
{
    #region Header
    /**
     * JsonException.cs
     *   Base class throwed by LitJSON when a parsing error occurs.
     *
     * The authors disclaim copyright to this source code. For more details, see
     * the COPYING file included with this distribution.
     **/
    #endregion
    
    
    using System;
    
    
    namespace LitJson
    {
        public class JsonException : ApplicationException
        {
            public JsonException () : base ()
            {
            }
    
            internal JsonException (ParserToken token) :
                base (String.Format (
                        "Invalid token '{0}' in input string", token))
            {
            }
    
            internal JsonException (ParserToken token,
                                    Exception inner_exception) :
                base (String.Format (
                        "Invalid token '{0}' in input string", token),
                    inner_exception)
            {
            }
    
            internal JsonException (int c) :
                base (String.Format (
                        "Invalid character '{0}' in input string", (char) c))
            {
            }
    
            internal JsonException (int c, Exception inner_exception) :
                base (String.Format (
                        "Invalid character '{0}' in input string", (char) c),
                    inner_exception)
            {
            }
    
    
            public JsonException (string message) : base (message)
            {
            }
    
            public JsonException (string message, Exception inner_exception) :
                base (message, inner_exception)
            {
            }
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonException.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonMapper.cs
namespace SqlServerSlackAPI
{
    #region Header
    /**
     * JsonMapper.cs
     *   JSON to .Net object and object to JSON conversions.
     *
     * The authors disclaim copyright to this source code. For more details, see
     * the COPYING file included with this distribution.
     **/
    #endregion
    
    
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    
    
    namespace LitJson
    {
        internal struct PropertyMetadata
        {
            public MemberInfo Info;
            public bool       IsField;
            public Type       Type;
        }
    
    
        internal struct ArrayMetadata
        {
            private Type element_type;
            private bool is_array;
            private bool is_list;
    
    
            public Type ElementType {
                get {
                    if (element_type == null)
                        return typeof (JsonData);
    
                    return element_type;
                }
    
                set { element_type = value; }
            }
    
            public bool IsArray {
                get { return is_array; }
                set { is_array = value; }
            }
    
            public bool IsList {
                get { return is_list; }
                set { is_list = value; }
            }
        }
    
    
        internal struct ObjectMetadata
        {
            private Type element_type;
            private bool is_dictionary;
    
            private IDictionary<string, PropertyMetadata> properties;
    
    
            public Type ElementType {
                get {
                    if (element_type == null)
                        return typeof (JsonData);
    
                    return element_type;
                }
    
                set { element_type = value; }
            }
    
            public bool IsDictionary {
                get { return is_dictionary; }
                set { is_dictionary = value; }
            }
    
            public IDictionary<string, PropertyMetadata> Properties {
                get { return properties; }
                set { properties = value; }
            }
        }
    
    
        internal delegate void ExporterFunc    (object obj, JsonWriter writer);
        public   delegate void ExporterFunc<T> (T obj, JsonWriter writer);
    
        internal delegate object ImporterFunc                (object input);
        public   delegate TValue ImporterFunc<TJson, TValue> (TJson input);
    
        public delegate IJsonWrapper WrapperFactory ();
    
    
        public class JsonMapper
        {
            #region Fields
            private static readonly int max_nesting_depth;
    
            private static readonly IFormatProvider datetime_format;
    
            private static readonly IDictionary<Type, ExporterFunc> base_exporters_table;
            private static readonly IDictionary<Type, ExporterFunc> custom_exporters_table;
    
            private static readonly IDictionary<Type,
                    IDictionary<Type, ImporterFunc>> base_importers_table;
            private static readonly IDictionary<Type,
                    IDictionary<Type, ImporterFunc>> custom_importers_table;
    
            private static readonly IDictionary<Type, ArrayMetadata> array_metadata;
            private static readonly object array_metadata_lock = new Object ();
    
            private static readonly IDictionary<Type,
                    IDictionary<Type, MethodInfo>> conv_ops;
            private static readonly object conv_ops_lock = new Object ();
    
            private static readonly IDictionary<Type, ObjectMetadata> object_metadata;
            private static readonly object object_metadata_lock = new Object ();
    
            private static readonly IDictionary<Type,
                    IList<PropertyMetadata>> type_properties;
            private static readonly object type_properties_lock = new Object ();
    
            private static readonly JsonWriter      static_writer;
            private static readonly object static_writer_lock = new Object ();
            #endregion
    
    
            #region Constructors
            static JsonMapper ()
            {
                max_nesting_depth = 100;
    
                array_metadata = new Dictionary<Type, ArrayMetadata> ();
                conv_ops = new Dictionary<Type, IDictionary<Type, MethodInfo>> ();
                object_metadata = new Dictionary<Type, ObjectMetadata> ();
                type_properties = new Dictionary<Type,
                                IList<PropertyMetadata>> ();
    
                static_writer = new JsonWriter ();
    
                datetime_format = DateTimeFormatInfo.InvariantInfo;
    
                base_exporters_table   = new Dictionary<Type, ExporterFunc> ();
                custom_exporters_table = new Dictionary<Type, ExporterFunc> ();
    
                base_importers_table = new Dictionary<Type,
                                     IDictionary<Type, ImporterFunc>> ();
                custom_importers_table = new Dictionary<Type,
                                       IDictionary<Type, ImporterFunc>> ();
    
                RegisterBaseExporters ();
                RegisterBaseImporters ();
            }
            #endregion
    
    
            #region Private Methods
            private static void AddArrayMetadata (Type type)
            {
                if (array_metadata.ContainsKey (type))
                    return;
    
                ArrayMetadata data = new ArrayMetadata ();
    
                data.IsArray = type.IsArray;
    
                if (type.GetInterface ("System.Collections.IList") != null)
                    data.IsList = true;
    
                foreach (PropertyInfo p_info in type.GetProperties ()) {
                    if (p_info.Name != "Item")
                        continue;
    
                    ParameterInfo[] parameters = p_info.GetIndexParameters ();
    
                    if (parameters.Length != 1)
                        continue;
    
                    if (parameters[0].ParameterType == typeof (int))
                        data.ElementType = p_info.PropertyType;
                }
    
                lock (array_metadata_lock) {
                    try {
                        array_metadata.Add (type, data);
                    } catch (ArgumentException) {
                        return;
                    }
                }
            }
    
            private static void AddObjectMetadata (Type type)
            {
                if (object_metadata.ContainsKey (type))
                    return;
    
                ObjectMetadata data = new ObjectMetadata ();
    
                if (type.GetInterface ("System.Collections.IDictionary") != null)
                    data.IsDictionary = true;
    
                data.Properties = new Dictionary<string, PropertyMetadata> ();
    
                foreach (PropertyInfo p_info in type.GetProperties ()) {
                    if (p_info.Name == "Item") {
                        ParameterInfo[] parameters = p_info.GetIndexParameters ();
    
                        if (parameters.Length != 1)
                            continue;
    
                        if (parameters[0].ParameterType == typeof (string))
                            data.ElementType = p_info.PropertyType;
    
                        continue;
                    }
    
                    PropertyMetadata p_data = new PropertyMetadata ();
                    p_data.Info = p_info;
                    p_data.Type = p_info.PropertyType;
    
                    data.Properties.Add (p_info.Name, p_data);
                }
    
                foreach (FieldInfo f_info in type.GetFields ()) {
                    PropertyMetadata p_data = new PropertyMetadata ();
                    p_data.Info = f_info;
                    p_data.IsField = true;
                    p_data.Type = f_info.FieldType;
    
                    data.Properties.Add (f_info.Name, p_data);
                }
    
                lock (object_metadata_lock) {
                    try {
                        object_metadata.Add (type, data);
                    } catch (ArgumentException) {
                        return;
                    }
                }
            }
    
            private static void AddTypeProperties (Type type)
            {
                if (type_properties.ContainsKey (type))
                    return;
    
                IList<PropertyMetadata> props = new List<PropertyMetadata> ();
    
                foreach (PropertyInfo p_info in type.GetProperties ()) {
                    if (p_info.Name == "Item")
                        continue;
    
                    PropertyMetadata p_data = new PropertyMetadata ();
                    p_data.Info = p_info;
                    p_data.IsField = false;
                    props.Add (p_data);
                }
    
                foreach (FieldInfo f_info in type.GetFields ()) {
                    PropertyMetadata p_data = new PropertyMetadata ();
                    p_data.Info = f_info;
                    p_data.IsField = true;
    
                    props.Add (p_data);
                }
    
                lock (type_properties_lock) {
                    try {
                        type_properties.Add (type, props);
                    } catch (ArgumentException) {
                        return;
                    }
                }
            }
    
            private static MethodInfo GetConvOp (Type t1, Type t2)
            {
                lock (conv_ops_lock) {
                    if (! conv_ops.ContainsKey (t1))
                        conv_ops.Add (t1, new Dictionary<Type, MethodInfo> ());
                }
    
                if (conv_ops[t1].ContainsKey (t2))
                    return conv_ops[t1][t2];
    
                MethodInfo op = t1.GetMethod (
                    "op_Implicit", new Type[] { t2 });
    
                lock (conv_ops_lock) {
                    try {
                        conv_ops[t1].Add (t2, op);
                    } catch (ArgumentException) {
                        return conv_ops[t1][t2];
                    }
                }
    
                return op;
            }
    
            private static object ReadValue (Type inst_type, JsonReader reader)
            {
                reader.Read ();
    
                if (reader.Token == JsonToken.ArrayEnd)
                    return null;
    
                Type underlying_type = Nullable.GetUnderlyingType(inst_type);
                Type value_type = underlying_type ?? inst_type;
    
                if (reader.Token == JsonToken.Null) {
                    if (inst_type.IsClass || underlying_type != null) {
                        return null;
                    }
    
                    throw new JsonException (String.Format (
                                "Can't assign null to an instance of type {0}",
                                inst_type));
                }
    
                if (reader.Token == JsonToken.Double ||
                    reader.Token == JsonToken.Int ||
                    reader.Token == JsonToken.Long ||
                    reader.Token == JsonToken.String ||
                    reader.Token == JsonToken.Boolean) {
    
                    Type json_type = reader.Value.GetType ();
    
                    if (value_type.IsAssignableFrom (json_type))
                        return reader.Value;
    
                    // If there's a custom importer that fits, use it
                    if (custom_importers_table.ContainsKey (json_type) &&
                        custom_importers_table[json_type].ContainsKey (
                            value_type)) {
    
                        ImporterFunc importer =
                            custom_importers_table[json_type][value_type];
    
                        return importer (reader.Value);
                    }
    
                    // Maybe there's a base importer that works
                    if (base_importers_table.ContainsKey (json_type) &&
                        base_importers_table[json_type].ContainsKey (
                            value_type)) {
    
                        ImporterFunc importer =
                            base_importers_table[json_type][value_type];
    
                        return importer (reader.Value);
                    }
    
                    // Maybe it's an enum
                    if (value_type.IsEnum)
                        return Enum.ToObject (value_type, reader.Value);
    
                    // Try using an implicit conversion operator
                    MethodInfo conv_op = GetConvOp (value_type, json_type);
    
                    if (conv_op != null)
                        return conv_op.Invoke (null,
                                               new object[] { reader.Value });
    
                    // No luck
                    throw new JsonException (String.Format (
                            "Can't assign value '{0}' (type {1}) to type {2}",
                            reader.Value, json_type, inst_type));
                }
    
                object instance = null;
    
                if (reader.Token == JsonToken.ArrayStart) {
    
                    AddArrayMetadata (inst_type);
                    ArrayMetadata t_data = array_metadata[inst_type];
    
                    if (! t_data.IsArray && ! t_data.IsList)
                        throw new JsonException (String.Format (
                                "Type {0} can't act as an array",
                                inst_type));
    
                    IList list;
                    Type elem_type;
    
                    if (! t_data.IsArray) {
                        list = (IList) Activator.CreateInstance (inst_type);
                        elem_type = t_data.ElementType;
                    } else {
                        list = new ArrayList ();
                        elem_type = inst_type.GetElementType ();
                    }
    
                    while (true) {
                        object item = ReadValue (elem_type, reader);
                        if (item == null && reader.Token == JsonToken.ArrayEnd)
                            break;
    
                        list.Add (item);
                    }
    
                    if (t_data.IsArray) {
                        int n = list.Count;
                        instance = Array.CreateInstance (elem_type, n);
    
                        for (int i = 0; i < n; i++)
                            ((Array) instance).SetValue (list[i], i);
                    } else
                        instance = list;
    
                } else if (reader.Token == JsonToken.ObjectStart) {
                    AddObjectMetadata (value_type);
                    ObjectMetadata t_data = object_metadata[value_type];
    
                    instance = Activator.CreateInstance (value_type);
    
                    while (true) {
                        reader.Read ();
    
                        if (reader.Token == JsonToken.ObjectEnd)
                            break;
    
                        string property = (string) reader.Value;
    
                        if (t_data.Properties.ContainsKey (property)) {
                            PropertyMetadata prop_data =
                                t_data.Properties[property];
    
                            if (prop_data.IsField) {
                                ((FieldInfo) prop_data.Info).SetValue (
                                    instance, ReadValue (prop_data.Type, reader));
                            } else {
                                PropertyInfo p_info =
                                    (PropertyInfo) prop_data.Info;
    
                                if (p_info.CanWrite)
                                    p_info.SetValue (
                                        instance,
                                        ReadValue (prop_data.Type, reader),
                                        null);
                                else
                                    ReadValue (prop_data.Type, reader);
                            }
    
                        } else {
                            if (! t_data.IsDictionary) {
    
                                if (! reader.SkipNonMembers) {
                                    throw new JsonException (String.Format (
                                            "The type {0} doesn't have the " +
                                            "property '{1}'",
                                            inst_type, property));
                                } else {
                                    ReadSkip (reader);
                                    continue;
                                }
                            }
    
                            ((IDictionary) instance).Add (
                                property, ReadValue (
                                    t_data.ElementType, reader));
                        }
    
                    }
    
                }
    
                return instance;
            }
    
            private static IJsonWrapper ReadValue (WrapperFactory factory,
                                                   JsonReader reader)
            {
                reader.Read ();
    
                if (reader.Token == JsonToken.ArrayEnd ||
                    reader.Token == JsonToken.Null)
                    return null;
    
                IJsonWrapper instance = factory ();
    
                if (reader.Token == JsonToken.String) {
                    instance.SetString ((string) reader.Value);
                    return instance;
                }
    
                if (reader.Token == JsonToken.Double) {
                    instance.SetDouble ((double) reader.Value);
                    return instance;
                }
    
                if (reader.Token == JsonToken.Int) {
                    instance.SetInt ((int) reader.Value);
                    return instance;
                }
    
                if (reader.Token == JsonToken.Long) {
                    instance.SetLong ((long) reader.Value);
                    return instance;
                }
    
                if (reader.Token == JsonToken.Boolean) {
                    instance.SetBoolean ((bool) reader.Value);
                    return instance;
                }
    
                if (reader.Token == JsonToken.ArrayStart) {
                    instance.SetJsonType (JsonType.Array);
    
                    while (true) {
                        IJsonWrapper item = ReadValue (factory, reader);
                        if (item == null && reader.Token == JsonToken.ArrayEnd)
                            break;
    
                        ((IList) instance).Add (item);
                    }
                }
                else if (reader.Token == JsonToken.ObjectStart) {
                    instance.SetJsonType (JsonType.Object);
    
                    while (true) {
                        reader.Read ();
    
                        if (reader.Token == JsonToken.ObjectEnd)
                            break;
    
                        string property = (string) reader.Value;
    
                        ((IDictionary) instance)[property] = ReadValue (
                            factory, reader);
                    }
    
                }
    
                return instance;
            }
    
            private static void ReadSkip (JsonReader reader)
            {
                ToWrapper (
                    delegate { return new JsonMockWrapper (); }, reader);
            }
    
            private static void RegisterBaseExporters ()
            {
                base_exporters_table[typeof (byte)] =
                    delegate (object obj, JsonWriter writer) {
                        writer.Write (Convert.ToInt32 ((byte) obj));
                    };
    
                base_exporters_table[typeof (char)] =
                    delegate (object obj, JsonWriter writer) {
                        writer.Write (Convert.ToString ((char) obj));
                    };
    
                base_exporters_table[typeof (DateTime)] =
                    delegate (object obj, JsonWriter writer) {
                        writer.Write (Convert.ToString ((DateTime) obj,
                                                        datetime_format));
                    };
    
                base_exporters_table[typeof (decimal)] =
                    delegate (object obj, JsonWriter writer) {
                        writer.Write ((decimal) obj);
                    };
    
                base_exporters_table[typeof (sbyte)] =
                    delegate (object obj, JsonWriter writer) {
                        writer.Write (Convert.ToInt32 ((sbyte) obj));
                    };
    
                base_exporters_table[typeof (short)] =
                    delegate (object obj, JsonWriter writer) {
                        writer.Write (Convert.ToInt32 ((short) obj));
                    };
    
                base_exporters_table[typeof (ushort)] =
                    delegate (object obj, JsonWriter writer) {
                        writer.Write (Convert.ToInt32 ((ushort) obj));
                    };
    
                base_exporters_table[typeof (uint)] =
                    delegate (object obj, JsonWriter writer) {
                        writer.Write (Convert.ToUInt64 ((uint) obj));
                    };
    
                base_exporters_table[typeof (ulong)] =
                    delegate (object obj, JsonWriter writer) {
                        writer.Write ((ulong) obj);
                    };
            }
    
            private static void RegisterBaseImporters ()
            {
                ImporterFunc importer;
    
                importer = delegate (object input) {
                    return Convert.ToByte ((int) input);
                };
                RegisterImporter (base_importers_table, typeof (int),
                                  typeof (byte), importer);
    
                importer = delegate (object input) {
                    return Convert.ToUInt64 ((int) input);
                };
                RegisterImporter (base_importers_table, typeof (int),
                                  typeof (ulong), importer);
    
                importer = delegate (object input) {
                    return Convert.ToSByte ((int) input);
                };
                RegisterImporter (base_importers_table, typeof (int),
                                  typeof (sbyte), importer);
    
                importer = delegate (object input) {
                    return Convert.ToInt16 ((int) input);
                };
                RegisterImporter (base_importers_table, typeof (int),
                                  typeof (short), importer);
    
                importer = delegate (object input) {
                    return Convert.ToUInt16 ((int) input);
                };
                RegisterImporter (base_importers_table, typeof (int),
                                  typeof (ushort), importer);
    
                importer = delegate (object input) {
                    return Convert.ToUInt32 ((int) input);
                };
                RegisterImporter (base_importers_table, typeof (int),
                                  typeof (uint), importer);
    
                importer = delegate (object input) {
                    return Convert.ToSingle ((int) input);
                };
                RegisterImporter (base_importers_table, typeof (int),
                                  typeof (float), importer);
    
                importer = delegate (object input) {
                    return Convert.ToDouble ((int) input);
                };
                RegisterImporter (base_importers_table, typeof (int),
                                  typeof (double), importer);
    
                importer = delegate (object input) {
                    return Convert.ToDecimal ((double) input);
                };
                RegisterImporter (base_importers_table, typeof (double),
                                  typeof (decimal), importer);
    
    
                importer = delegate (object input) {
                    return Convert.ToUInt32 ((long) input);
                };
                RegisterImporter (base_importers_table, typeof (long),
                                  typeof (uint), importer);
    
                importer = delegate (object input) {
                    return Convert.ToChar ((string) input);
                };
                RegisterImporter (base_importers_table, typeof (string),
                                  typeof (char), importer);
    
                importer = delegate (object input) {
                    return Convert.ToDateTime ((string) input, datetime_format);
                };
                RegisterImporter (base_importers_table, typeof (string),
                                  typeof (DateTime), importer);
            }
    
            private static void RegisterImporter (
                IDictionary<Type, IDictionary<Type, ImporterFunc>> table,
                Type json_type, Type value_type, ImporterFunc importer)
            {
                if (! table.ContainsKey (json_type))
                    table.Add (json_type, new Dictionary<Type, ImporterFunc> ());
    
                table[json_type][value_type] = importer;
            }
    
            private static void WriteValue (object obj, JsonWriter writer,
                                            bool writer_is_private,
                                            int depth)
            {
                if (depth > max_nesting_depth)
                    throw new JsonException (
                        String.Format ("Max allowed object depth reached while " +
                                       "trying to export from type {0}",
                                       obj.GetType ()));
    
                if (obj == null) {
                    writer.Write (null);
                    return;
                }
    
                if (obj is IJsonWrapper) {
                    if (writer_is_private)
                        writer.TextWriter.Write (((IJsonWrapper) obj).ToJson ());
                    else
                        ((IJsonWrapper) obj).ToJson (writer);
    
                    return;
                }
    
                if (obj is String) {
                    writer.Write ((string) obj);
                    return;
                }
    
                if (obj is Double) {
                    writer.Write ((double) obj);
                    return;
                }
    
                if (obj is Int32) {
                    writer.Write ((int) obj);
                    return;
                }
    
                if (obj is Boolean) {
                    writer.Write ((bool) obj);
                    return;
                }
    
                if (obj is Int64) {
                    writer.Write ((long) obj);
                    return;
                }
    
                if (obj is Array) {
                    writer.WriteArrayStart ();
    
                    foreach (object elem in (Array) obj)
                        WriteValue (elem, writer, writer_is_private, depth + 1);
    
                    writer.WriteArrayEnd ();
    
                    return;
                }
    
                if (obj is IList) {
                    writer.WriteArrayStart ();
                    foreach (object elem in (IList) obj)
                        WriteValue (elem, writer, writer_is_private, depth + 1);
                    writer.WriteArrayEnd ();
    
                    return;
                }
    
                if (obj is IDictionary) {
                    writer.WriteObjectStart ();
                    foreach (DictionaryEntry entry in (IDictionary) obj) {
                        writer.WritePropertyName ((string) entry.Key);
                        WriteValue (entry.Value, writer, writer_is_private,
                                    depth + 1);
                    }
                    writer.WriteObjectEnd ();
    
                    return;
                }
    
                Type obj_type = obj.GetType ();
    
                // See if there's a custom exporter for the object
                if (custom_exporters_table.ContainsKey (obj_type)) {
                    ExporterFunc exporter = custom_exporters_table[obj_type];
                    exporter (obj, writer);
    
                    return;
                }
    
                // If not, maybe there's a base exporter
                if (base_exporters_table.ContainsKey (obj_type)) {
                    ExporterFunc exporter = base_exporters_table[obj_type];
                    exporter (obj, writer);
    
                    return;
                }
    
                // Last option, let's see if it's an enum
                if (obj is Enum) {
                    Type e_type = Enum.GetUnderlyingType (obj_type);
    
                    if (e_type == typeof (long)
                        || e_type == typeof (uint)
                        || e_type == typeof (ulong))
                        writer.Write ((ulong) obj);
                    else
                        writer.Write ((int) obj);
    
                    return;
                }
    
                // Okay, so it looks like the input should be exported as an
                // object
                AddTypeProperties (obj_type);
                IList<PropertyMetadata> props = type_properties[obj_type];
    
                writer.WriteObjectStart ();
                foreach (PropertyMetadata p_data in props) {
                    if (p_data.IsField) {
                        writer.WritePropertyName (p_data.Info.Name);
                        WriteValue (((FieldInfo) p_data.Info).GetValue (obj),
                                    writer, writer_is_private, depth + 1);
                    }
                    else {
                        PropertyInfo p_info = (PropertyInfo) p_data.Info;
    
                        if (p_info.CanRead) {
                            writer.WritePropertyName (p_data.Info.Name);
                            WriteValue (p_info.GetValue (obj, null),
                                        writer, writer_is_private, depth + 1);
                        }
                    }
                }
                writer.WriteObjectEnd ();
            }
            #endregion
    
    
            public static string ToJson (object obj)
            {
                lock (static_writer_lock) {
                    static_writer.Reset ();
    
                    WriteValue (obj, static_writer, true, 0);
    
                    return static_writer.ToString ();
                }
            }
    
            public static void ToJson (object obj, JsonWriter writer)
            {
                WriteValue (obj, writer, false, 0);
            }
    
            public static JsonData ToObject (JsonReader reader)
            {
                return (JsonData) ToWrapper (
                    delegate { return new JsonData (); }, reader);
            }
    
            public static JsonData ToObject (TextReader reader)
            {
                JsonReader json_reader = new JsonReader (reader);
    
                return (JsonData) ToWrapper (
                    delegate { return new JsonData (); }, json_reader);
            }
    
            public static JsonData ToObject (string json)
            {
                return (JsonData) ToWrapper (
                    delegate { return new JsonData (); }, json);
            }
    
            public static T ToObject<T> (JsonReader reader)
            {
                return (T) ReadValue (typeof (T), reader);
            }
    
            public static T ToObject<T> (TextReader reader)
            {
                JsonReader json_reader = new JsonReader (reader);
    
                return (T) ReadValue (typeof (T), json_reader);
            }
    
            public static T ToObject<T> (string json)
            {
                JsonReader reader = new JsonReader (json);
    
                return (T) ReadValue (typeof (T), reader);
            }
    
            public static IJsonWrapper ToWrapper (WrapperFactory factory,
                                                  JsonReader reader)
            {
                return ReadValue (factory, reader);
            }
    
            public static IJsonWrapper ToWrapper (WrapperFactory factory,
                                                  string json)
            {
                JsonReader reader = new JsonReader (json);
    
                return ReadValue (factory, reader);
            }
    
            public static void RegisterExporter<T> (ExporterFunc<T> exporter)
            {
                ExporterFunc exporter_wrapper =
                    delegate (object obj, JsonWriter writer) {
                        exporter ((T) obj, writer);
                    };
    
                custom_exporters_table[typeof (T)] = exporter_wrapper;
            }
    
            public static void RegisterImporter<TJson, TValue> (
                ImporterFunc<TJson, TValue> importer)
            {
                ImporterFunc importer_wrapper =
                    delegate (object input) {
                        return importer ((TJson) input);
                    };
    
                RegisterImporter (custom_importers_table, typeof (TJson),
                                  typeof (TValue), importer_wrapper);
            }
    
            public static void UnregisterExporters ()
            {
                custom_exporters_table.Clear ();
            }
    
            public static void UnregisterImporters ()
            {
                custom_importers_table.Clear ();
            }
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonMapper.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonReader.cs?
namespace SqlServerSlackAPI
{
    #region Header
    /**
     * JsonReader.cs
     *   Stream-like access to JSON text.
     *
     * The authors disclaim copyright to this source code. For more details, see
     * the COPYING file included with this distribution.
     **/
    #endregion
    
    
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    
    
    namespace LitJson
    {
        public enum JsonToken
        {
            None,
    
            ObjectStart,
            PropertyName,
            ObjectEnd,
    
            ArrayStart,
            ArrayEnd,
    
            Int,
            Long,
            Double,
    
            String,
    
            Boolean,
            Null
        }
    
    
        public class JsonReader
        {
            #region Fields
            private static readonly IDictionary<int, IDictionary<int, int[]>> parse_table;
    
            private Stack<int>    automaton_stack;
            private int           current_input;
            private int           current_symbol;
            private bool          end_of_json;
            private bool          end_of_input;
            private Lexer         lexer;
            private bool          parser_in_string;
            private bool          parser_return;
            private bool          read_started;
            private TextReader    reader;
            private bool          reader_is_owned;
            private bool          skip_non_members;
            private object        token_value;
            private JsonToken     token;
            #endregion
    
    
            #region Public Properties
            public bool AllowComments {
                get { return lexer.AllowComments; }
                set { lexer.AllowComments = value; }
            }
    
            public bool AllowSingleQuotedStrings {
                get { return lexer.AllowSingleQuotedStrings; }
                set { lexer.AllowSingleQuotedStrings = value; }
            }
    
            public bool SkipNonMembers {
                get { return skip_non_members; }
                set { skip_non_members = value; }
            }
    
            public bool EndOfInput {
                get { return end_of_input; }
            }
    
            public bool EndOfJson {
                get { return end_of_json; }
            }
    
            public JsonToken Token {
                get { return token; }
            }
    
            public object Value {
                get { return token_value; }
            }
            #endregion
    
    
            #region Constructors
            static JsonReader ()
            {
                parse_table = PopulateParseTable ();
            }
    
            public JsonReader (string json_text) :
                this (new StringReader (json_text), true)
            {
            }
    
            public JsonReader (TextReader reader) :
                this (reader, false)
            {
            }
    
            private JsonReader (TextReader reader, bool owned)
            {
                if (reader == null)
                    throw new ArgumentNullException ("reader");
    
                parser_in_string = false;
                parser_return    = false;
    
                read_started = false;
                automaton_stack = new Stack<int> ();
                automaton_stack.Push ((int) ParserToken.End);
                automaton_stack.Push ((int) ParserToken.Text);
    
                lexer = new Lexer (reader);
    
                end_of_input = false;
                end_of_json  = false;
    
                skip_non_members = true;
    
                this.reader = reader;
                reader_is_owned = owned;
            }
            #endregion
    
    
            #region Static Methods
            private static IDictionary<int, IDictionary<int, int[]>> PopulateParseTable ()
            {
                // See section A.2. of the manual for details
                IDictionary<int, IDictionary<int, int[]>> parse_table = new Dictionary<int, IDictionary<int, int[]>> ();
        
                TableAddRow (parse_table, ParserToken.Array);
                TableAddCol (parse_table, ParserToken.Array, '[',
                                '[',
                                (int) ParserToken.ArrayPrime);
        
                TableAddRow (parse_table, ParserToken.ArrayPrime);
                TableAddCol (parse_table, ParserToken.ArrayPrime, '"',
                                (int) ParserToken.Value,
        
                                (int) ParserToken.ValueRest,
                                ']');
                TableAddCol (parse_table, ParserToken.ArrayPrime, '[',
                                (int) ParserToken.Value,
                                (int) ParserToken.ValueRest,
                                ']');
                TableAddCol (parse_table, ParserToken.ArrayPrime, ']',
                                ']');
                TableAddCol (parse_table, ParserToken.ArrayPrime, '{',
                                (int) ParserToken.Value,
                                (int) ParserToken.ValueRest,
                                ']');
                TableAddCol (parse_table, ParserToken.ArrayPrime, (int) ParserToken.Number,
                                (int) ParserToken.Value,
                                (int) ParserToken.ValueRest,
                                ']');
                TableAddCol (parse_table, ParserToken.ArrayPrime, (int) ParserToken.True,
                                (int) ParserToken.Value,
                                (int) ParserToken.ValueRest,
                                ']');
                TableAddCol (parse_table, ParserToken.ArrayPrime, (int) ParserToken.False,
                                (int) ParserToken.Value,
                                (int) ParserToken.ValueRest,
                                ']');
                TableAddCol (parse_table, ParserToken.ArrayPrime, (int) ParserToken.Null,
                                (int) ParserToken.Value,
                                (int) ParserToken.ValueRest,
                                ']');
        
                TableAddRow (parse_table, ParserToken.Object);
                TableAddCol (parse_table, ParserToken.Object, '{',
                                '{',
                                (int) ParserToken.ObjectPrime);
        
                TableAddRow (parse_table, ParserToken.ObjectPrime);
                TableAddCol (parse_table, ParserToken.ObjectPrime, '"',
                                (int) ParserToken.Pair,
                                (int) ParserToken.PairRest,
                                '}');
                TableAddCol (parse_table, ParserToken.ObjectPrime, '}',
                                '}');
        
                TableAddRow (parse_table, ParserToken.Pair);
                TableAddCol (parse_table, ParserToken.Pair, '"',
                                (int) ParserToken.String,
                                ':',
                                (int) ParserToken.Value);
        
                TableAddRow (parse_table, ParserToken.PairRest);
                TableAddCol (parse_table, ParserToken.PairRest, ',',
                                ',',
                                (int) ParserToken.Pair,
                                (int) ParserToken.PairRest);
                TableAddCol (parse_table, ParserToken.PairRest, '}',
                                (int) ParserToken.Epsilon);
        
                TableAddRow (parse_table, ParserToken.String);
                TableAddCol (parse_table, ParserToken.String, '"',
                                '"',
                                (int) ParserToken.CharSeq,
                                '"');
        
                TableAddRow (parse_table, ParserToken.Text);
                TableAddCol (parse_table, ParserToken.Text, '[',
                                (int) ParserToken.Array);
                TableAddCol (parse_table, ParserToken.Text, '{',
                                (int) ParserToken.Object);
        
                TableAddRow (parse_table, ParserToken.Value);
                TableAddCol (parse_table, ParserToken.Value, '"',
                                (int) ParserToken.String);
                TableAddCol (parse_table, ParserToken.Value, '[',
                                (int) ParserToken.Array);
                TableAddCol (parse_table, ParserToken.Value, '{',
                                (int) ParserToken.Object);
                TableAddCol (parse_table, ParserToken.Value, (int) ParserToken.Number,
                                (int) ParserToken.Number);
                TableAddCol (parse_table, ParserToken.Value, (int) ParserToken.True,
                                (int) ParserToken.True);
                TableAddCol (parse_table, ParserToken.Value, (int) ParserToken.False,
                                (int) ParserToken.False);
                TableAddCol (parse_table, ParserToken.Value, (int) ParserToken.Null,
                                (int) ParserToken.Null);
        
                TableAddRow (parse_table, ParserToken.ValueRest);
                TableAddCol (parse_table, ParserToken.ValueRest, ',',
                                ',',
                                (int) ParserToken.Value,
                                (int) ParserToken.ValueRest);
                TableAddCol (parse_table, ParserToken.ValueRest, ']',
                                (int) ParserToken.Epsilon);
        
                return parse_table;
            }
    
            private static void TableAddCol (IDictionary<int, IDictionary<int, int[]>> parse_table, ParserToken row, int col,
                                             params int[] symbols)
            {
                parse_table[(int) row].Add (col, symbols);
            }
    
            private static void TableAddRow (IDictionary<int, IDictionary<int, int[]>> parse_table, ParserToken rule)
            {
                parse_table.Add ((int) rule, new Dictionary<int, int[]> ());
            }
            #endregion
    
    
            #region Private Methods
            private void ProcessNumber (string number)
            {
                if (number.IndexOf ('.') != -1 ||
                    number.IndexOf ('e') != -1 ||
                    number.IndexOf ('E') != -1) {
    
                    double n_double;
                    if (Double.TryParse (number, out n_double)) {
                        token = JsonToken.Double;
                        token_value = n_double;
    
                        return;
                    }
                }
    
                int n_int32;
                if (Int32.TryParse (number, out n_int32)) {
                    token = JsonToken.Int;
                    token_value = n_int32;
    
                    return;
                }
    
                long n_int64;
                if (Int64.TryParse (number, out n_int64)) {
                    token = JsonToken.Long;
                    token_value = n_int64;
    
                    return;
                }
    
                ulong n_uint64;
                if (UInt64.TryParse(number, out n_uint64))
                {
                    token = JsonToken.Long;
                    token_value = n_uint64;
    
                    return;
                }
    
                // Shouldn't happen, but just in case, return something
                token = JsonToken.Int;
                token_value = 0;
            }
    
            private void ProcessSymbol ()
            {
                if (current_symbol == '[')  {
                    token = JsonToken.ArrayStart;
                    parser_return = true;
    
                } else if (current_symbol == ']')  {
                    token = JsonToken.ArrayEnd;
                    parser_return = true;
    
                } else if (current_symbol == '{')  {
                    token = JsonToken.ObjectStart;
                    parser_return = true;
    
                } else if (current_symbol == '}')  {
                    token = JsonToken.ObjectEnd;
                    parser_return = true;
    
                } else if (current_symbol == '"')  {
                    if (parser_in_string) {
                        parser_in_string = false;
    
                        parser_return = true;
    
                    } else {
                        if (token == JsonToken.None)
                            token = JsonToken.String;
    
                        parser_in_string = true;
                    }
    
                } else if (current_symbol == (int) ParserToken.CharSeq) {
                    token_value = lexer.StringValue;
    
                } else if (current_symbol == (int) ParserToken.False)  {
                    token = JsonToken.Boolean;
                    token_value = false;
                    parser_return = true;
    
                } else if (current_symbol == (int) ParserToken.Null)  {
                    token = JsonToken.Null;
                    parser_return = true;
    
                } else if (current_symbol == (int) ParserToken.Number)  {
                    ProcessNumber (lexer.StringValue);
    
                    parser_return = true;
    
                } else if (current_symbol == (int) ParserToken.Pair)  {
                    token = JsonToken.PropertyName;
    
                } else if (current_symbol == (int) ParserToken.True)  {
                    token = JsonToken.Boolean;
                    token_value = true;
                    parser_return = true;
    
                }
            }
    
            private bool ReadToken ()
            {
                if (end_of_input)
                    return false;
    
                lexer.NextToken ();
    
                if (lexer.EndOfInput) {
                    Close ();
    
                    return false;
                }
    
                current_input = lexer.Token;
    
                return true;
            }
            #endregion
    
    
            public void Close ()
            {
                if (end_of_input)
                    return;
    
                end_of_input = true;
                end_of_json  = true;
    
                if (reader_is_owned)
                    reader.Close ();
    
                reader = null;
            }
    
            public bool Read ()
            {
                if (end_of_input)
                    return false;
    
                if (end_of_json) {
                    end_of_json = false;
                    automaton_stack.Clear ();
                    automaton_stack.Push ((int) ParserToken.End);
                    automaton_stack.Push ((int) ParserToken.Text);
                }
    
                parser_in_string = false;
                parser_return    = false;
    
                token       = JsonToken.None;
                token_value = null;
    
                if (! read_started) {
                    read_started = true;
    
                    if (! ReadToken ())
                        return false;
                }
    
    
                int[] entry_symbols;
    
                while (true) {
                    if (parser_return) {
                        if (automaton_stack.Peek () == (int) ParserToken.End)
                            end_of_json = true;
    
                        return true;
                    }
    
                    current_symbol = automaton_stack.Pop ();
    
                    ProcessSymbol ();
    
                    if (current_symbol == current_input) {
                        if (! ReadToken ()) {
                            if (automaton_stack.Peek () != (int) ParserToken.End)
                                throw new JsonException (
                                    "Input doesn't evaluate to proper JSON text");
    
                            if (parser_return)
                                return true;
    
                            return false;
                        }
    
                        continue;
                    }
    
                    try {
    
                        entry_symbols =
                            parse_table[current_symbol][current_input];
    
                    } catch (KeyNotFoundException e) {
                        throw new JsonException ((ParserToken) current_input, e);
                    }
    
                    if (entry_symbols[0] == (int) ParserToken.Epsilon)
                        continue;
    
                    for (int i = entry_symbols.Length - 1; i >= 0; i--)
                        automaton_stack.Push (entry_symbols[i]);
                }
            }
    
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonReader.cs?
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonWriter.cs
namespace SqlServerSlackAPI
{
    #region Header
    /**
     * JsonWriter.cs
     *   Stream-like facility to output JSON text.
     *
     * The authors disclaim copyright to this source code. For more details, see
     * the COPYING file included with this distribution.
     **/
    #endregion
    
    
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    
    
    namespace LitJson
    {
        internal enum Condition
        {
            InArray,
            InObject,
            NotAProperty,
            Property,
            Value
        }
    
        internal class WriterContext
        {
            public int  Count;
            public bool InArray;
            public bool InObject;
            public bool ExpectingValue;
            public int  Padding;
        }
    
        public class JsonWriter
        {
            #region Fields
            private static readonly NumberFormatInfo number_format;
    
            private WriterContext        context;
            private Stack<WriterContext> ctx_stack;
            private bool                 has_reached_end;
            private char[]               hex_seq;
            private int                  indentation;
            private int                  indent_value;
            private StringBuilder        inst_string_builder;
            private bool                 pretty_print;
            private bool                 validate;
            private TextWriter           writer;
            #endregion
    
    
            #region Properties
            public int IndentValue {
                get { return indent_value; }
                set {
                    indentation = (indentation / indent_value) * value;
                    indent_value = value;
                }
            }
    
            public bool PrettyPrint {
                get { return pretty_print; }
                set { pretty_print = value; }
            }
    
            public TextWriter TextWriter {
                get { return writer; }
            }
    
            public bool Validate {
                get { return validate; }
                set { validate = value; }
            }
            #endregion
    
    
            #region Constructors
            static JsonWriter ()
            {
                number_format = NumberFormatInfo.InvariantInfo;
            }
    
            public JsonWriter ()
            {
                inst_string_builder = new StringBuilder ();
                writer = new StringWriter (inst_string_builder);
    
                Init ();
            }
    
            public JsonWriter (StringBuilder sb) :
                this (new StringWriter (sb))
            {
            }
    
            public JsonWriter (TextWriter writer)
            {
                if (writer == null)
                    throw new ArgumentNullException ("writer");
    
                this.writer = writer;
    
                Init ();
            }
            #endregion
    
    
            #region Private Methods
            private void DoValidation (Condition cond)
            {
                if (! context.ExpectingValue)
                    context.Count++;
    
                if (! validate)
                    return;
    
                if (has_reached_end)
                    throw new JsonException (
                        "A complete JSON symbol has already been written");
    
                switch (cond) {
                case Condition.InArray:
                    if (! context.InArray)
                        throw new JsonException (
                            "Can't close an array here");
                    break;
    
                case Condition.InObject:
                    if (! context.InObject || context.ExpectingValue)
                        throw new JsonException (
                            "Can't close an object here");
                    break;
    
                case Condition.NotAProperty:
                    if (context.InObject && ! context.ExpectingValue)
                        throw new JsonException (
                            "Expected a property");
                    break;
    
                case Condition.Property:
                    if (! context.InObject || context.ExpectingValue)
                        throw new JsonException (
                            "Can't add a property here");
                    break;
    
                case Condition.Value:
                    if (! context.InArray &&
                        (! context.InObject || ! context.ExpectingValue))
                        throw new JsonException (
                            "Can't add a value here");
    
                    break;
                }
            }
    
            private void Init ()
            {
                has_reached_end = false;
                hex_seq = new char[4];
                indentation = 0;
                indent_value = 4;
                pretty_print = false;
                validate = true;
    
                ctx_stack = new Stack<WriterContext> ();
                context = new WriterContext ();
                ctx_stack.Push (context);
            }
    
            private static void IntToHex (int n, char[] hex)
            {
                int num;
    
                for (int i = 0; i < 4; i++) {
                    num = n % 16;
    
                    if (num < 10)
                        hex[3 - i] = (char) ('0' + num);
                    else
                        hex[3 - i] = (char) ('A' + (num - 10));
    
                    n >>= 4;
                }
            }
    
            private void Indent ()
            {
                if (pretty_print)
                    indentation += indent_value;
            }
    
    
            private void Put (string str)
            {
                if (pretty_print && ! context.ExpectingValue)
                    for (int i = 0; i < indentation; i++)
                        writer.Write (' ');
    
                writer.Write (str);
            }
    
            private void PutNewline ()
            {
                PutNewline (true);
            }
    
            private void PutNewline (bool add_comma)
            {
                if (add_comma && ! context.ExpectingValue &&
                    context.Count > 1)
                    writer.Write (',');
    
                if (pretty_print && ! context.ExpectingValue)
                    writer.Write ('\n');
            }
    
            private void PutString (string str)
            {
                Put (String.Empty);
    
                writer.Write ('"');
    
                int n = str.Length;
                for (int i = 0; i < n; i++) {
                    switch (str[i]) {
                    case '\n':
                        writer.Write ("\\n");
                        continue;
    
                    case '\r':
                        writer.Write ("\\r");
                        continue;
    
                    case '\t':
                        writer.Write ("\\t");
                        continue;
    
                    case '"':
                    case '\\':
                        writer.Write ('\\');
                        writer.Write (str[i]);
                        continue;
    
                    case '\f':
                        writer.Write ("\\f");
                        continue;
    
                    case '\b':
                        writer.Write ("\\b");
                        continue;
                    }
    
                    if ((int) str[i] >= 32 && (int) str[i] <= 126) {
                        writer.Write (str[i]);
                        continue;
                    }
    
                    // Default, turn into a \uXXXX sequence
                    IntToHex ((int) str[i], hex_seq);
                    writer.Write ("\\u");
                    writer.Write (hex_seq);
                }
    
                writer.Write ('"');
            }
    
            private void Unindent ()
            {
                if (pretty_print)
                    indentation -= indent_value;
            }
            #endregion
    
    
            public override string ToString ()
            {
                if (inst_string_builder == null)
                    return String.Empty;
    
                return inst_string_builder.ToString ();
            }
    
            public void Reset ()
            {
                has_reached_end = false;
    
                ctx_stack.Clear ();
                context = new WriterContext ();
                ctx_stack.Push (context);
    
                if (inst_string_builder != null)
                    inst_string_builder.Remove (0, inst_string_builder.Length);
            }
    
            public void Write (bool boolean)
            {
                DoValidation (Condition.Value);
                PutNewline ();
    
                Put (boolean ? "true" : "false");
    
                context.ExpectingValue = false;
            }
    
            public void Write (decimal number)
            {
                DoValidation (Condition.Value);
                PutNewline ();
    
                Put (Convert.ToString (number, number_format));
    
                context.ExpectingValue = false;
            }
    
            public void Write (double number)
            {
                DoValidation (Condition.Value);
                PutNewline ();
    
                string str = Convert.ToString (number, number_format);
                Put (str);
    
                if (str.IndexOf ('.') == -1 &&
                    str.IndexOf ('E') == -1)
                    writer.Write (".0");
    
                context.ExpectingValue = false;
            }
    
            public void Write (int number)
            {
                DoValidation (Condition.Value);
                PutNewline ();
    
                Put (Convert.ToString (number, number_format));
    
                context.ExpectingValue = false;
            }
    
            public void Write (long number)
            {
                DoValidation (Condition.Value);
                PutNewline ();
    
                Put (Convert.ToString (number, number_format));
    
                context.ExpectingValue = false;
            }
    
            public void Write (string str)
            {
                DoValidation (Condition.Value);
                PutNewline ();
    
                if (str == null)
                    Put ("null");
                else
                    PutString (str);
    
                context.ExpectingValue = false;
            }
    
            [CLSCompliant(false)]
            public void Write (ulong number)
            {
                DoValidation (Condition.Value);
                PutNewline ();
    
                Put (Convert.ToString (number, number_format));
    
                context.ExpectingValue = false;
            }
    
            public void WriteArrayEnd ()
            {
                DoValidation (Condition.InArray);
                PutNewline (false);
    
                ctx_stack.Pop ();
                if (ctx_stack.Count == 1)
                    has_reached_end = true;
                else {
                    context = ctx_stack.Peek ();
                    context.ExpectingValue = false;
                }
    
                Unindent ();
                Put ("]");
            }
    
            public void WriteArrayStart ()
            {
                DoValidation (Condition.NotAProperty);
                PutNewline ();
    
                Put ("[");
    
                context = new WriterContext ();
                context.InArray = true;
                ctx_stack.Push (context);
    
                Indent ();
            }
    
            public void WriteObjectEnd ()
            {
                DoValidation (Condition.InObject);
                PutNewline (false);
    
                ctx_stack.Pop ();
                if (ctx_stack.Count == 1)
                    has_reached_end = true;
                else {
                    context = ctx_stack.Peek ();
                    context.ExpectingValue = false;
                }
    
                Unindent ();
                Put ("}");
            }
    
            public void WriteObjectStart ()
            {
                DoValidation (Condition.NotAProperty);
                PutNewline ();
    
                Put ("{");
    
                context = new WriterContext ();
                context.InObject = true;
                ctx_stack.Push (context);
    
                Indent ();
            }
    
            public void WritePropertyName (string property_name)
            {
                DoValidation (Condition.Property);
                PutNewline ();
    
                PutString (property_name);
    
                if (pretty_print) {
                    if (property_name.Length > context.Padding)
                        context.Padding = property_name.Length;
    
                    for (int i = context.Padding - property_name.Length;
                         i >= 0; i--)
                        writer.Write (' ');
    
                    writer.Write (": ");
                } else
                    writer.Write (':');
    
                context.ExpectingValue = true;
            }
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonWriter.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonMockWrapper.cs
namespace SqlServerSlackAPI
{
    #region Header
    /**
     * JsonMockWrapper.cs
     *   Mock object implementing IJsonWrapper, to facilitate actions like
     *   skipping data more efficiently.
     *
     * The authors disclaim copyright to this source code. For more details, see
     * the COPYING file included with this distribution.
     **/
    #endregion
    
    
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    
    
    namespace LitJson
    {
        public class JsonMockWrapper : IJsonWrapper
        {
            public bool IsArray   { get { return false; } }
            public bool IsBoolean { get { return false; } }
            public bool IsDouble  { get { return false; } }
            public bool IsInt     { get { return false; } }
            public bool IsLong    { get { return false; } }
            public bool IsObject  { get { return false; } }
            public bool IsString  { get { return false; } }
    
            public bool     GetBoolean ()  { return false; }
            public double   GetDouble ()   { return 0.0; }
            public int      GetInt ()      { return 0; }
            public JsonType GetJsonType () { return JsonType.None; }
            public long     GetLong ()     { return 0L; }
            public string   GetString ()   { return ""; }
    
            public void SetBoolean  (bool val)      {}
            public void SetDouble   (double val)    {}
            public void SetInt      (int val)       {}
            public void SetJsonType (JsonType type) {}
            public void SetLong     (long val)      {}
            public void SetString   (string val)    {}
    
            public string ToJson ()                  { return ""; }
            public void   ToJson (JsonWriter writer) {}
    
    
            bool IList.IsFixedSize { get { return true; } }
            bool IList.IsReadOnly  { get { return true; } }
    
            object IList.this[int index] {
                get { return null; }
                set {}
            }
    
            int  IList.Add (object value)       { return 0; }
            void IList.Clear ()                 {}
            bool IList.Contains (object value)  { return false; }
            int  IList.IndexOf (object value)   { return -1; }
            void IList.Insert (int i, object v) {}
            void IList.Remove (object value)    {}
            void IList.RemoveAt (int index)     {}
    
    
            int    ICollection.Count          { get { return 0; } }
            bool   ICollection.IsSynchronized { get { return false; } }
            object ICollection.SyncRoot       { get { return null; } }
    
            void ICollection.CopyTo (Array array, int index) {}
    
    
            IEnumerator IEnumerable.GetEnumerator () { return null; }
    
    
            bool IDictionary.IsFixedSize { get { return true; } }
            bool IDictionary.IsReadOnly  { get { return true; } }
    
            ICollection IDictionary.Keys   { get { return null; } }
            ICollection IDictionary.Values { get { return null; } }
    
            object IDictionary.this[object key] {
                get { return null; }
                set {}
            }
    
            void IDictionary.Add (object k, object v) {}
            void IDictionary.Clear ()                 {}
            bool IDictionary.Contains (object key)    { return false; }
            void IDictionary.Remove (object key)      {}
    
            IDictionaryEnumerator IDictionary.GetEnumerator () { return null; }
    
    
            object IOrderedDictionary.this[int idx] {
                get { return null; }
                set {}
            }
    
            IDictionaryEnumerator IOrderedDictionary.GetEnumerator () {
                return null;
            }
            void IOrderedDictionary.Insert   (int i, object k, object v) {}
            void IOrderedDictionary.RemoveAt (int i) {}
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonMockWrapper.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/Lexer.cs
namespace SqlServerSlackAPI
{
    #region Header
    /**
     * Lexer.cs
     *   JSON lexer implementation based on a finite state machine.
     *
     * The authors disclaim copyright to this source code. For more details, see
     * the COPYING file included with this distribution.
     **/
    #endregion
    
    
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    
    
    namespace LitJson
    {
        internal class FsmContext
        {
            public bool  Return;
            public int   NextState;
            public Lexer L;
            public int   StateStack;
        }
    
    
        internal class Lexer
        {
            #region Fields
            private delegate bool StateHandler (FsmContext ctx);
    
            private static readonly int[]          fsm_return_table;
            private static readonly StateHandler[] fsm_handler_table;
    
            private bool          allow_comments;
            private bool          allow_single_quoted_strings;
            private bool          end_of_input;
            private FsmContext    fsm_context;
            private int           input_buffer;
            private int           input_char;
            private TextReader    reader;
            private int           state;
            private StringBuilder string_buffer;
            private string        string_value;
            private int           token;
            private int           unichar;
            #endregion
    
    
            #region Properties
            public bool AllowComments {
                get { return allow_comments; }
                set { allow_comments = value; }
            }
    
            public bool AllowSingleQuotedStrings {
                get { return allow_single_quoted_strings; }
                set { allow_single_quoted_strings = value; }
            }
    
            public bool EndOfInput {
                get { return end_of_input; }
            }
    
            public int Token {
                get { return token; }
            }
    
            public string StringValue {
                get { return string_value; }
            }
            #endregion
    
    
            #region Constructors
            static Lexer ()
            {
                PopulateFsmTables (out fsm_handler_table, out fsm_return_table);
            }
    
            public Lexer (TextReader reader)
            {
                allow_comments = true;
                allow_single_quoted_strings = true;
    
                input_buffer = 0;
                string_buffer = new StringBuilder (128);
                state = 1;
                end_of_input = false;
                this.reader = reader;
    
                fsm_context = new FsmContext ();
                fsm_context.L = this;
            }
            #endregion
    
    
            #region Static Methods
            private static int HexValue (int digit)
            {
                switch (digit) {
                case 'a':
                case 'A':
                    return 10;
    
                case 'b':
                case 'B':
                    return 11;
    
                case 'c':
                case 'C':
                    return 12;
    
                case 'd':
                case 'D':
                    return 13;
    
                case 'e':
                case 'E':
                    return 14;
    
                case 'f':
                case 'F':
                    return 15;
    
                default:
                    return digit - '0';
                }
            }
    
            private static void PopulateFsmTables (out StateHandler[] fsm_handler_table, out int[] fsm_return_table)
            {
                // See section A.1. of the manual for details of the finite
                // state machine.
                fsm_handler_table = new StateHandler[28] {
                    State1,
                    State2,
                    State3,
                    State4,
                    State5,
                    State6,
                    State7,
                    State8,
                    State9,
                    State10,
                    State11,
                    State12,
                    State13,
                    State14,
                    State15,
                    State16,
                    State17,
                    State18,
                    State19,
                    State20,
                    State21,
                    State22,
                    State23,
                    State24,
                    State25,
                    State26,
                    State27,
                    State28
                };
    
                fsm_return_table = new int[28] {
                    (int) ParserToken.Char,
                    0,
                    (int) ParserToken.Number,
                    (int) ParserToken.Number,
                    0,
                    (int) ParserToken.Number,
                    0,
                    (int) ParserToken.Number,
                    0,
                    0,
                    (int) ParserToken.True,
                    0,
                    0,
                    0,
                    (int) ParserToken.False,
                    0,
                    0,
                    (int) ParserToken.Null,
                    (int) ParserToken.CharSeq,
                    (int) ParserToken.Char,
                    0,
                    0,
                    (int) ParserToken.CharSeq,
                    (int) ParserToken.Char,
                    0,
                    0,
                    0,
                    0
                };
            }
    
            private static char ProcessEscChar (int esc_char)
            {
                switch (esc_char) {
                case '"':
                case '\'':
                case '\\':
                case '/':
                    return Convert.ToChar (esc_char);
    
                case 'n':
                    return '\n';
    
                case 't':
                    return '\t';
    
                case 'r':
                    return '\r';
    
                case 'b':
                    return '\b';
    
                case 'f':
                    return '\f';
    
                default:
                    // Unreachable
                    return '?';
                }
            }
    
            private static bool State1 (FsmContext ctx)
            {
                while (ctx.L.GetChar ()) {
                    if (ctx.L.input_char == ' ' ||
                        ctx.L.input_char >= '\t' && ctx.L.input_char <= '\r')
                        continue;
    
                    if (ctx.L.input_char >= '1' && ctx.L.input_char <= '9') {
                        ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                        ctx.NextState = 3;
                        return true;
                    }
    
                    switch (ctx.L.input_char) {
                    case '"':
                        ctx.NextState = 19;
                        ctx.Return = true;
                        return true;
    
                    case ',':
                    case ':':
                    case '[':
                    case ']':
                    case '{':
                    case '}':
                        ctx.NextState = 1;
                        ctx.Return = true;
                        return true;
    
                    case '-':
                        ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                        ctx.NextState = 2;
                        return true;
    
                    case '0':
                        ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                        ctx.NextState = 4;
                        return true;
    
                    case 'f':
                        ctx.NextState = 12;
                        return true;
    
                    case 'n':
                        ctx.NextState = 16;
                        return true;
    
                    case 't':
                        ctx.NextState = 9;
                        return true;
    
                    case '\'':
                        if (! ctx.L.allow_single_quoted_strings)
                            return false;
    
                        ctx.L.input_char = '"';
                        ctx.NextState = 23;
                        ctx.Return = true;
                        return true;
    
                    case '/':
                        if (! ctx.L.allow_comments)
                            return false;
    
                        ctx.NextState = 25;
                        return true;
    
                    default:
                        return false;
                    }
                }
    
                return true;
            }
    
            private static bool State2 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                if (ctx.L.input_char >= '1' && ctx.L.input_char<= '9') {
                    ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                    ctx.NextState = 3;
                    return true;
                }
    
                switch (ctx.L.input_char) {
                case '0':
                    ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                    ctx.NextState = 4;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State3 (FsmContext ctx)
            {
                while (ctx.L.GetChar ()) {
                    if (ctx.L.input_char >= '0' && ctx.L.input_char <= '9') {
                        ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                        continue;
                    }
    
                    if (ctx.L.input_char == ' ' ||
                        ctx.L.input_char >= '\t' && ctx.L.input_char <= '\r') {
                        ctx.Return = true;
                        ctx.NextState = 1;
                        return true;
                    }
    
                    switch (ctx.L.input_char) {
                    case ',':
                    case ']':
                    case '}':
                        ctx.L.UngetChar ();
                        ctx.Return = true;
                        ctx.NextState = 1;
                        return true;
    
                    case '.':
                        ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                        ctx.NextState = 5;
                        return true;
    
                    case 'e':
                    case 'E':
                        ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                        ctx.NextState = 7;
                        return true;
    
                    default:
                        return false;
                    }
                }
                return true;
            }
    
            private static bool State4 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                if (ctx.L.input_char == ' ' ||
                    ctx.L.input_char >= '\t' && ctx.L.input_char <= '\r') {
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;
                }
    
                switch (ctx.L.input_char) {
                case ',':
                case ']':
                case '}':
                    ctx.L.UngetChar ();
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;
    
                case '.':
                    ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                    ctx.NextState = 5;
                    return true;
    
                case 'e':
                case 'E':
                    ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                    ctx.NextState = 7;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State5 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                if (ctx.L.input_char >= '0' && ctx.L.input_char <= '9') {
                    ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                    ctx.NextState = 6;
                    return true;
                }
    
                return false;
            }
    
            private static bool State6 (FsmContext ctx)
            {
                while (ctx.L.GetChar ()) {
                    if (ctx.L.input_char >= '0' && ctx.L.input_char <= '9') {
                        ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                        continue;
                    }
    
                    if (ctx.L.input_char == ' ' ||
                        ctx.L.input_char >= '\t' && ctx.L.input_char <= '\r') {
                        ctx.Return = true;
                        ctx.NextState = 1;
                        return true;
                    }
    
                    switch (ctx.L.input_char) {
                    case ',':
                    case ']':
                    case '}':
                        ctx.L.UngetChar ();
                        ctx.Return = true;
                        ctx.NextState = 1;
                        return true;
    
                    case 'e':
                    case 'E':
                        ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                        ctx.NextState = 7;
                        return true;
    
                    default:
                        return false;
                    }
                }
    
                return true;
            }
    
            private static bool State7 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                if (ctx.L.input_char >= '0' && ctx.L.input_char<= '9') {
                    ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                    ctx.NextState = 8;
                    return true;
                }
    
                switch (ctx.L.input_char) {
                case '+':
                case '-':
                    ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                    ctx.NextState = 8;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State8 (FsmContext ctx)
            {
                while (ctx.L.GetChar ()) {
                    if (ctx.L.input_char >= '0' && ctx.L.input_char<= '9') {
                        ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                        continue;
                    }
    
                    if (ctx.L.input_char == ' ' ||
                        ctx.L.input_char >= '\t' && ctx.L.input_char<= '\r') {
                        ctx.Return = true;
                        ctx.NextState = 1;
                        return true;
                    }
    
                    switch (ctx.L.input_char) {
                    case ',':
                    case ']':
                    case '}':
                        ctx.L.UngetChar ();
                        ctx.Return = true;
                        ctx.NextState = 1;
                        return true;
    
                    default:
                        return false;
                    }
                }
    
                return true;
            }
    
            private static bool State9 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                switch (ctx.L.input_char) {
                case 'r':
                    ctx.NextState = 10;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State10 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                switch (ctx.L.input_char) {
                case 'u':
                    ctx.NextState = 11;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State11 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                switch (ctx.L.input_char) {
                case 'e':
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State12 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                switch (ctx.L.input_char) {
                case 'a':
                    ctx.NextState = 13;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State13 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                switch (ctx.L.input_char) {
                case 'l':
                    ctx.NextState = 14;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State14 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                switch (ctx.L.input_char) {
                case 's':
                    ctx.NextState = 15;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State15 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                switch (ctx.L.input_char) {
                case 'e':
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State16 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                switch (ctx.L.input_char) {
                case 'u':
                    ctx.NextState = 17;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State17 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                switch (ctx.L.input_char) {
                case 'l':
                    ctx.NextState = 18;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State18 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                switch (ctx.L.input_char) {
                case 'l':
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State19 (FsmContext ctx)
            {
                while (ctx.L.GetChar ()) {
                    switch (ctx.L.input_char) {
                    case '"':
                        ctx.L.UngetChar ();
                        ctx.Return = true;
                        ctx.NextState = 20;
                        return true;
    
                    case '\\':
                        ctx.StateStack = 19;
                        ctx.NextState = 21;
                        return true;
    
                    default:
                        ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                        continue;
                    }
                }
    
                return true;
            }
    
            private static bool State20 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                switch (ctx.L.input_char) {
                case '"':
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State21 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                switch (ctx.L.input_char) {
                case 'u':
                    ctx.NextState = 22;
                    return true;
    
                case '"':
                case '\'':
                case '/':
                case '\\':
                case 'b':
                case 'f':
                case 'n':
                case 'r':
                case 't':
                    ctx.L.string_buffer.Append (
                        ProcessEscChar (ctx.L.input_char));
                    ctx.NextState = ctx.StateStack;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State22 (FsmContext ctx)
            {
                int counter = 0;
                int mult    = 4096;
    
                ctx.L.unichar = 0;
    
                while (ctx.L.GetChar ()) {
    
                    if (ctx.L.input_char >= '0' && ctx.L.input_char <= '9' ||
                        ctx.L.input_char >= 'A' && ctx.L.input_char <= 'F' ||
                        ctx.L.input_char >= 'a' && ctx.L.input_char <= 'f') {
    
                        ctx.L.unichar += HexValue (ctx.L.input_char) * mult;
    
                        counter++;
                        mult /= 16;
    
                        if (counter == 4) {
                            ctx.L.string_buffer.Append (
                                Convert.ToChar (ctx.L.unichar));
                            ctx.NextState = ctx.StateStack;
                            return true;
                        }
    
                        continue;
                    }
    
                    return false;
                }
    
                return true;
            }
    
            private static bool State23 (FsmContext ctx)
            {
                while (ctx.L.GetChar ()) {
                    switch (ctx.L.input_char) {
                    case '\'':
                        ctx.L.UngetChar ();
                        ctx.Return = true;
                        ctx.NextState = 24;
                        return true;
    
                    case '\\':
                        ctx.StateStack = 23;
                        ctx.NextState = 21;
                        return true;
    
                    default:
                        ctx.L.string_buffer.Append ((char) ctx.L.input_char);
                        continue;
                    }
                }
    
                return true;
            }
    
            private static bool State24 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                switch (ctx.L.input_char) {
                case '\'':
                    ctx.L.input_char = '"';
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State25 (FsmContext ctx)
            {
                ctx.L.GetChar ();
    
                switch (ctx.L.input_char) {
                case '*':
                    ctx.NextState = 27;
                    return true;
    
                case '/':
                    ctx.NextState = 26;
                    return true;
    
                default:
                    return false;
                }
            }
    
            private static bool State26 (FsmContext ctx)
            {
                while (ctx.L.GetChar ()) {
                    if (ctx.L.input_char == '\n') {
                        ctx.NextState = 1;
                        return true;
                    }
                }
    
                return true;
            }
    
            private static bool State27 (FsmContext ctx)
            {
                while (ctx.L.GetChar ()) {
                    if (ctx.L.input_char == '*') {
                        ctx.NextState = 28;
                        return true;
                    }
                }
    
                return true;
            }
    
            private static bool State28 (FsmContext ctx)
            {
                while (ctx.L.GetChar ()) {
                    if (ctx.L.input_char == '*')
                        continue;
    
                    if (ctx.L.input_char == '/') {
                        ctx.NextState = 1;
                        return true;
                    }
    
                    ctx.NextState = 27;
                    return true;
                }
    
                return true;
            }
            #endregion
    
    
            private bool GetChar ()
            {
                if ((input_char = NextChar ()) != -1)
                    return true;
    
                end_of_input = true;
                return false;
            }
    
            private int NextChar ()
            {
                if (input_buffer != 0) {
                    int tmp = input_buffer;
                    input_buffer = 0;
    
                    return tmp;
                }
    
                return reader.Read ();
            }
    
            public bool NextToken ()
            {
                StateHandler handler;
                fsm_context.Return = false;
    
                while (true) {
                    handler = fsm_handler_table[state - 1];
    
                    if (! handler (fsm_context))
                        throw new JsonException (input_char);
    
                    if (end_of_input)
                        return false;
    
                    if (fsm_context.Return) {
                        string_value = string_buffer.ToString ();
                        string_buffer.Remove (0, string_buffer.Length);
                        token = fsm_return_table[state - 1];
    
                        if (token == (int) ParserToken.Char)
                            token = input_char;
    
                        state = fsm_context.NextState;
    
                        return true;
                    }
    
                    state = fsm_context.NextState;
                }
            }
    
            private void UngetChar ()
            {
                input_buffer = input_char;
            }
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/Lexer.cs
// ############################################################################

// ############################################################################
// @@@ BEGIN_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/ParserToken.cs
namespace SqlServerSlackAPI
{
    #region Header
    /**
     * ParserToken.cs
     *   Internal representation of the tokens used by the lexer and the parser.
     *
     * The authors disclaim copyright to this source code. For more details, see
     * the COPYING file included with this distribution.
     **/
    #endregion
    
    
    namespace LitJson
    {
        internal enum ParserToken
        {
            // Lexer tokens (see section A.1.1. of the manual)
            None = System.Char.MaxValue + 1,
            Number,
            True,
            False,
            Null,
            CharSeq,
            // Single char
            Char,
    
            // Parser Rules (see section A.2.1 of the manual)
            Text,
            Object,
            ObjectPrime,
            Pair,
            PairRest,
            Array,
            ArrayPrime,
            Value,
            ValueRest,
            String,
    
            // End of input
            End,
    
            // The empty rule
            Epsilon
        }
    }
}
// @@@ END_INCLUDE: https://raw.github.com/WCOMAB/litjson/master/src/LitJson/ParserToken.cs
// ############################################################################
// ############################################################################
// Certains directives such as #define and // Resharper comments has to be 
// moved to bottom in order to work properly    
// ############################################################################
// ############################################################################
namespace SqlServerSlackAPI.Include
{
    static partial class MetaData
    {
        public const string RootPath        = @"https://raw.github.com/";
        public const string IncludeDate     = @"2015-01-19T15:44:49";

        public const string Include_0       = @"https://raw.github.com/WCOMAB/litjson/master/src/LitJson/IJsonWrapper.cs";
        public const string Include_1       = @"https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonData.cs";
        public const string Include_2       = @"https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonException.cs";
        public const string Include_3       = @"https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonMapper.cs";
        public const string Include_4       = @"https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonReader.cs?";
        public const string Include_5       = @"https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonWriter.cs";
        public const string Include_6       = @"https://raw.github.com/WCOMAB/litjson/master/src/LitJson/JsonMockWrapper.cs";
        public const string Include_7       = @"https://raw.github.com/WCOMAB/litjson/master/src/LitJson/Lexer.cs";
        public const string Include_8       = @"https://raw.github.com/WCOMAB/litjson/master/src/LitJson/ParserToken.cs";
    }
}
// ############################################################################



