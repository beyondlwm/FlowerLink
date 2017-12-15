using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MyDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    where TValue : new()
{
    public new TValue this[TKey key] {
        get
        {
            TValue ret;
            if (!ContainsKey(key))
            {
                ret = new TValue();
                Add(key, ret);
            }
            else
            {
                TryGetValue(key, out ret);
            }
            return ret;
        }
        set
        {
            Add(key, value);
        }
    }

}
