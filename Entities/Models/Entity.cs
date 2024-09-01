using Entities.LinkModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace Entities.Models
{
    public class Entity: IDictionary<string, object>
    {
        private readonly IDictionary<string, object> _expando;

        public Entity()
        {
            _expando = new ExpandoObject();
        }


        public void Add(string key, object value)
        {
            _expando.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _expando.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _expando.Keys; }
        }

        public bool Remove(string key)
        {
            return _expando.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _expando.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get { return _expando.Values; }
        }

        public object this[string key]
        {
            get
            {
                return _expando[key];
            }
            set
            {
                _expando[key] = value;
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _expando.Add(item);
        }

        public void Clear()
        {
            _expando.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _expando.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            _expando.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _expando.Count; }
        }

        public bool IsReadOnly
        {
            get { return _expando.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _expando.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _expando.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        private void WriteLinksToXml(string key,object value, XmlWriter writer)
        {
            writer.WriteStartElement(key);
            if (value.GetType()==typeof(List<Link>))
            {
                foreach (var val in value as List<Link>)
                {
                    writer.WriteStartElement(nameof(Link));
                    WriteLinksToXml(nameof(val.Href), val.Href, writer);
                    WriteLinksToXml(nameof(val.Rel), val.Rel, writer);
                    WriteLinksToXml(nameof(val.Method), val.Method, writer);
                    writer.WriteEndElement();
                }
            }
            else
            {
                writer.WriteString(value.ToString());
            }
            writer.WriteEndElement();
        }
    }
}
