using System.Collections.Generic;
using System.Linq;

namespace CommonCameraUtil;

public class StackList<T>
{
	private List<T> _list;

	public StackList()
	{
		_list = new List<T>();
	}

	public T Pop()
	{
		if (_list.Count == 0)
		{
			return default(T);
		}
		else
		{
			var obj = _list.Last();
			_list.RemoveAt(_list.Count - 1);
			return obj;
		}
	}

	public void Add(T obj)
	{
		if (_list.Contains(obj))
			_list.Remove(obj);
		_list.Add(obj);
	}

	public T Peek() => _list.LastOrDefault();

	public void Remove(T obj)
	{
		if (_list.Contains(obj))
			_list.Remove(obj);
	}

	public void Clear() => _list.Clear();
}
