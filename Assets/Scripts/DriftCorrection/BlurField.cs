using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.DriftCorrection {
	// Blurs values in a two dimensional gridded coordinate system.
	// Takes in pre-defined data and approximates data of blank grids.
	// Can be utilized to generate a heat map of an arbitrary value type.
	// See: http://cs.stackexchange.com/questions/10178.
	public class BlurField<V> {
		// Defines a characteristic of a type of value.
		// This is desigend to support primitive/final types.
		public interface ValueType {
			// Retrieves "zero" of this value type.
			// Used as an initial value when folding a sum of values.
			// Used as a default value in blank grids.
			// e.g.: 0.
			V Identity();

			// Retrieves a sum of two values of this type.
			// Used to combine influence from multiple pre-defined data.
			// Used to generate a blank grid's value from surrounding grids.
			// e.g.: +.
			V Sum(V v1, V v2);

			// Retrieves a "blend" of two values of this type.
			// Used to combine influence from multiple pre-defined data.
			// Definition varies upon desired behavior; Mathf.Max() may make sense.
			V Blend(V v1, V v2);

			// Retrieves a product of two values of this type.
			// Used to map out influence from pre-defined data around its grid.
			// Used to generate a blank grid's value from surrounding grids.
			// e.g.: *.
			V Multiply(V v1, float time);

			// Retrieves radius of influence.
			// Retrieves zero if this value is not defined by user.
			float Radius(V v);
		}

		class Grid {
			public V Value { get; set; }
			public bool Static { get; set; } // True if this grid is pre-defined.
		}

		ValueType vDef;
		Grid[,] grids;

		public bool HasConstructed { get; private set; }

		int xSize { get { return grids.GetLength(0); } }
		int ySize { get { return grids.GetLength(1); } }

		public BlurField(ValueType vDef, int xSize, int ySize) {
			this.vDef = vDef;

			grids = new Grid[xSize,  ySize];
		}

		public BlurField(ValueType vDef, IntVector2 size)
			: this(vDef, size.x, size.y) { }

		// Pre-defines a value and its influence radius at specified grid.
		// E.g.: To make the grid affect two rows of its surrounding grids, make radius 2.
		// Make sure to call UpdateGrids() to apply the change to the map.
		public void DefineGrid(int x, int y, V value) {
			if (HasConstructed) {
				throw new InvalidOperationException(
					"Grids must be defined before constructing field.");
			}

			if (0 <= x && x < xSize && 0 <= y && y < ySize) {
				grids[x, y] = new Grid {
					Value = value,
					Static = true,
				};
			} else {
				Debug.LogAssertionFormat("Index out of range: ({0}, {1})", x, y);
			}
		}

		public void DefineGrid(IntVector2 position, V value) {
			DefineGrid(position.x, position.y, value);
		}
		
		// Maps out pre-defined values on the field.
		public void MapOutPredefinedGrids() {
			if (HasConstructed) {
				throw new InvalidOperationException(
					"Field must only be constructed once.");
			}

			UpdateStaticGrids();

			HasConstructed = true;
		}

		public void Blur(int times = 1) {
			if (!HasConstructed) {
				throw new InvalidOperationException(
					"Field must be constructed before applying blur.");
			}

			for (int _ = 0; _ < times; ++_) {
				UpdateDynamicGrids();
			}
		}

		// Retrieves a value at specified position in the field.
		// If the grid hasn't been defined/calculated, null will be returned.
		public V GetValue(int x, int y) {
			if (0 <= x && x < xSize && 0 <= y && y < ySize) {
				Grid g = grids[x, y];
				return (g == null) ? default(V) : g.Value;
			} else {
				Debug.LogAssertionFormat("Index out of range: ({0}, {1})", x, y);
				return default(V);
			}
		}

		public V GetValue(IntVector2 position) {
			return GetValue(position.x, position.y);
		}

		public IEnumerable<IntVector2> Grids() {
			for (int x = 0; x < xSize; ++x) {
				for (int y = 0; y < ySize; ++y) {
					yield return new IntVector2(x, y);
				}
			}
		}

		Grid IdentityGrid() {
			return new Grid {
				Value = vDef.Identity(),
			};
		}

		// Maps out pre-defined grids.
		void UpdateStaticGrids() {
			for (int x = 0; x < xSize; ++x) {
				for (int y = 0; y < ySize; ++y) {
					Grid grid = grids[x, y];
					if (grid != null && grid.Static) {
						float radius = vDef.Radius(grid.Value);
						int _radius = (int)radius;
						for (int _x = x - _radius; _x <= x + _radius; ++_x) {
							for (int _y = y - _radius; _y <= y + _radius; ++_y) {
								bool xOver = _x < 0 || xSize <= _x;
								bool yOver = _y < 0 || ySize <= _y;
								bool itself = _x == x && _y == y;
								if (!xOver && !yOver && !itself) {
									float distance = Distance(x, y, _x, _y);
									if (distance <= radius) {
										float time = Mathf.Max(1f - distance / radius, 0f);
                                        V value = vDef.Multiply(grid.Value, time);
										Grid _grid = grids[_x, _y];
										if (_grid == null) {
											_grid = IdentityGrid();
											_grid.Value = value;
										} else {
											_grid.Value = vDef.Blend(_grid.Value, value);
										}
										grids[_x, _y] = _grid;
                                    }
								}
							}
						}
					}
				}
			}
		}

		// Blurs out grids.
		void UpdateDynamicGrids() {
			for (int x = 0; x < xSize; ++x) {
				for (int y = 0; y < ySize; ++y) {
					Grid grid = grids[x, y] ?? IdentityGrid();
					if (!grid.Static) {
						int count = 0;
						V sum = vDef.Identity();
						for (int sx = x - 1; sx <= x + 1; ++sx) {
							for (int sy = y - 1; sy <= y + 1; ++sy) {
								bool xOver = sx < 0 || xSize <= sx;
								bool yOver = sy < 0 || ySize <= sy;
								bool itself = sx == x && sy == y;
								if (!xOver && !yOver && !itself) {
									Grid source = grids[sx, sy];
									if (source != null) {
										sum = vDef.Sum(sum, source.Value);
										count += 1;
									}
								}
							}
						}
						if (count >= 2) {
							V averageValue = vDef.Multiply(sum, 1f / count);
							grid.Value = averageValue;
						}
					}
					grids[x, y] = grid;
				}
			}
		}

		static float Distance(float x1, float y1, float x2, float y2) {
			return Vector2.Distance(new Vector2(x1, y1), new Vector2(x2, y2));
		}
	}
}
