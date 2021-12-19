using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net
{
/*    public class ValueEncoderPipeline : ICollection<IValueEncoder<object, string>>, IValueEncoder<object, string>
    {
        List<IValueEncoder<object, string>> _innerList;
        public ValueEncoderPipeline(params IValueEncoder<object, string>[] values)
        {
            this._innerList = new List<IValueEncoder<object, string>>();
        }

        IValueEncoder<object, string> _current;
        public IValueEncoder<object, string> Current 
        {
            get
            {
                return _current;
            }
            private set
            {
                _current = value;
                _currentIndex = _innerList.IndexOf(_current);
            }
        }

        int _currentIndex;
        public int CurrentIndex
        {
            get
            {
                return _currentIndex;
            }
            set
            {
                _currentIndex = value;
                _current = _innerList[_currentIndex];
            }
        }

        public byte[] CurrentBytes
        {
            get
            {
                return ConvertObjectToBytes(_current);
            }
        }

        public object CurrentObject
        {
            get
            {
                return ConvertBytesToObject(CurrentBytes);
            }
        }

        public int Count => this._innerList.Count;

        public bool IsReadOnly => false;

        public void Add(IValueEncoder<object, string> item)
        {
            _innerList.Add(item);
        }

        public void Clear()
        {
            _innerList.Clear();
        }

        public bool Contains(IValueEncoder<object, string> item)
        {
            return _innerList.Contains(item);
        }

        public void CopyTo(IValueEncoder<object, string>[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }

        public byte[] Encode(object value)
        {
            byte[] result = null;
            foreach (IValueEncoder<object, string> valueEncoder in this)
            {
                result = Current.Encode(value);
            }
            return result;
        }

        public IEnumerator<IValueEncoder<object, string>> GetEnumerator()
        {
            for (int i = 0; i < _innerList.Count; i++)
            {
                Current = _innerList[i];
                yield return Current;
            }
        }

        /// <summary>
        /// Using the current encoder converts the specified byte array to an object.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public object ConvertBytesToObject(byte[] bytes)
        {
            return Current.ConvertBytesToObject(bytes);
        }

        public bool Remove(IValueEncoder<object, string> item)
        {
            return _innerList.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public byte[] ConvertObjectToBytes(object value)
        {
            return Current.ConvertObjectToBytes(value);
        }

        public string ConvertByteArrayToString(byte[] bytes)
        {
            return Current.ConvertByteArrayToString(bytes);
        }

        public string ConvertObjectToString(object value)
        {
            return Current.ConvertObjectToString(value);
        }

        public string Decode(byte[] encoded)
        {
            return Current.Decode(encoded);
        }
    }*/
}
