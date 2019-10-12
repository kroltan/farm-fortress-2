using System.Collections.Generic;
using UnityEngine;

namespace LD45
{
	public static class Extensions
	{
		public static void Deconstruct<TKey, TValue>(
			this KeyValuePair<TKey, TValue> self,
			out TKey key,
			out TValue value
		) {
			key = self.Key;
			value = self.Value;
		}

		public static bool Contains(this LayerMask self, int layer) {
			return (self.value & 1 << layer) == 1;
		}

		public static Color Change(this Color color, float? r = null, float? g = null, float? b = null, float? a = null) {
			if (r.HasValue) {
				color.r = r.Value;
			}

			if (g.HasValue) {
				color.g = g.Value;
			}

			if (b.HasValue) {
				color.b = b.Value;
			}

			if (a.HasValue) {
				color.a = a.Value;
			}

			return color;
		}
	}

	public class SceneAttribute : PropertyAttribute { }

	public class LayerAttribute : PropertyAttribute { }
}
