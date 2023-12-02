using System;
using System.Collections.Generic;
using System.Linq;

namespace Classes.Domain.Extensions;

public static class EnumerableExtensions
{
	public static IReadOnlyCollection<T> AsReadOnlyCollection<T>(this IEnumerable<T>? source)
	{
		if (source is null) return Array.Empty<T>();
		return source as IReadOnlyCollection<T> ?? source.ToList();
	}

	public static IEnumerable<T> AsList<T>(this T? source) =>
		source is null ? Enumerable.Empty<T>() : new List<T> { source };
}