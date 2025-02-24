// Copyright (c) DotSpatial Team. All rights reserved.
// Licensed under the MIT license. See License.txt file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using DotSpatial.Data;
using DotSpatial.Serialization;

namespace DotSpatial.Symbology
{
    /// <summary>
    /// Scheme with colors support.
    /// </summary>
    [Serializable]
    public class ColorScheme : Scheme, IColorScheme
    {
        #region Fields

        private ColorCategoryCollection _categories;
        private float _opacity;

        #endregion

        #region  Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorScheme"/> class.
        /// </summary>
        public ColorScheme()
        {
            Configure();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorScheme"/> class using a predefined color scheme and the minimum and maximum specified
        /// from the raster itself.
        /// </summary>
        /// <param name="schemeType">The predefined scheme to use.</param>
        /// <param name="raster">The raster to obtain the minimum and maximum settings from.</param>
        public ColorScheme(ColorSchemeType schemeType, IRaster raster)
        {
            Configure();
            ApplyScheme(schemeType, raster);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorScheme"/> class, applying the specified color scheme, and using the minimum and maximum values indicated.
        /// </summary>
        /// <param name="schemeType">The predefined color scheme.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        public ColorScheme(ColorSchemeType schemeType, double min, double max)
        {
            Configure();
            ApplyScheme(schemeType, min, max);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the raster categories.
        /// </summary>
        [Serialize("Categories")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ColorCategoryCollection Categories
        {
            get
            {
                return _categories;
            }

            set
            {
                if (_categories != null) _categories.Scheme = null;
                _categories = value;
                if (_categories != null) _categories.Scheme = this;
            }
        }

        /// <summary>
        /// Gets or sets the raster editor settings associated with this scheme.
        /// </summary>
        [Serialize("EditorSettings")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new RasterEditorSettings EditorSettings
        {
            get
            {
                return base.EditorSettings as RasterEditorSettings;
            }

            set
            {
                base.EditorSettings = value;
            }
        }

        /// <summary>
        /// Gets or sets the floating point value for the opacity.
        /// </summary>
        [Serialize("Opacity")]
        public float Opacity
        {
            get
            {
                return _opacity;
            }

            set
            {
                _opacity = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified category.
        /// </summary>
        /// <param name="category">Category that gets added.</param>
        public override void AddCategory(ICategory category)
        {
            if (category is IColorCategory cc) _categories.Add(cc);
        }

        /// <summary>
        /// Applies the specified color scheme and uses the specified raster to define the
        /// minimum and maximum to use for the scheme.
        /// </summary>
        /// <param name="schemeType">Color scheme that gets applied.</param>
        /// <param name="raster">Raster that is used to define the minimum and maximum for the scheme.</param>
        public void ApplyScheme(ColorSchemeType schemeType, IRaster raster)
        {
            double min, max;
            if (!raster.IsInRam)
            {
                GetValues(raster);
                min = Statistics.Minimum;
                max = Statistics.Maximum;
            }
            else
            {
                min = raster.Minimum;
                max = raster.Maximum;
            }

            ApplyScheme(schemeType, min, max);
        }

        /// <summary>
        /// Applies the specified color scheme and uses the specified raster to define the
        /// minimum and maximum to use for the scheme.
        /// </summary>
        /// <param name="schemeType">ColorSchemeType.</param>
        /// <param name="min">THe minimum value to use for the scheme.</param>
        /// <param name="max">THe maximum value to use for the scheme.</param>
        public void ApplyScheme(ColorSchemeType schemeType, double min, double max)
        {
            if (Categories == null)
            {
                Categories = new ColorCategoryCollection(this);
            }
            else
            {
                Categories.Clear();
            }

            IColorCategory eqCat = null, low = null, high = null;
            if (min == max)
            {
                // Create one category
                eqCat = new ColorCategory(min, max) { Range = { MaxIsInclusive = true, MinIsInclusive = true } };
                eqCat.ApplyMinMax(EditorSettings);
                Categories.Add(eqCat);
            }
            else
            {
                // Create two categories
                low = new ColorCategory(min, (min + max) / 2) { Range = { MaxIsInclusive = true } };
                high = new ColorCategory((min + max) / 2, max) { Range = { MaxIsInclusive = true } };
                low.ApplyMinMax(EditorSettings);
                high.ApplyMinMax(EditorSettings);
                Categories.Add(low);
                Categories.Add(high);
            }

            Color lowColor, midColor, highColor;
            int alpha = Utils.ByteRange(Convert.ToInt32(_opacity * 255F));
            switch (schemeType)
            {
                case ColorSchemeType.SummerMountains:
                    lowColor = Color.FromArgb(alpha, 10, 100, 10);
                    midColor = Color.FromArgb(alpha, 153, 125, 25);
                    highColor = Color.FromArgb(alpha, 255, 255, 255);
                    break;
                case ColorSchemeType.FallLeaves:
                    lowColor = Color.FromArgb(alpha, 10, 100, 10);
                    midColor = Color.FromArgb(alpha, 199, 130, 61);
                    highColor = Color.FromArgb(alpha, 241, 220, 133);
                    break;
                case ColorSchemeType.Desert:
                    lowColor = Color.FromArgb(alpha, 211, 206, 97);
                    midColor = Color.FromArgb(alpha, 139, 120, 112);
                    highColor = Color.FromArgb(alpha, 255, 255, 255);
                    break;
                case ColorSchemeType.Glaciers:
                    lowColor = Color.FromArgb(alpha, 105, 171, 224);
                    midColor = Color.FromArgb(alpha, 162, 234, 240);
                    highColor = Color.FromArgb(alpha, 255, 255, 255);
                    break;
                case ColorSchemeType.Meadow:
                    lowColor = Color.FromArgb(alpha, 68, 128, 71);
                    midColor = Color.FromArgb(alpha, 43, 91, 30);
                    highColor = Color.FromArgb(alpha, 167, 220, 168);
                    break;
                case ColorSchemeType.ValleyFires:
                    lowColor = Color.FromArgb(alpha, 164, 0, 0);
                    midColor = Color.FromArgb(alpha, 255, 128, 64);
                    highColor = Color.FromArgb(alpha, 255, 255, 191);
                    break;
                case ColorSchemeType.DeadSea:
                    lowColor = Color.FromArgb(alpha, 51, 137, 208);
                    midColor = Color.FromArgb(alpha, 226, 227, 166);
                    highColor = Color.FromArgb(alpha, 151, 146, 117);
                    break;
                case ColorSchemeType.Highway:
                    lowColor = Color.FromArgb(alpha, 51, 137, 208);
                    midColor = Color.FromArgb(alpha, 214, 207, 124);
                    highColor = Color.FromArgb(alpha, 54, 152, 69);
                    break;
                default:
                    lowColor = midColor = highColor = Color.Transparent;
                    break;
            }

            if (eqCat != null)
            {
                eqCat.LowColor = eqCat.HighColor = lowColor;
            }
            else
            {
                Debug.Assert(low != null, "low may not be null");
                Debug.Assert(high != null, "high may not be null");

                low.LowColor = lowColor;
                low.HighColor = midColor;
                high.LowColor = midColor;
                high.HighColor = highColor;
            }

            OnItemChanged(this);
        }

        /// <summary>
        /// Clears the categories.
        /// </summary>
        public override void ClearCategories()
        {
            _categories.Clear();
        }

        /// <summary>
        /// Creates the categories for this scheme based on statistics and values
        /// sampled from the specified raster.
        /// </summary>
        /// <param name="raster">The raster to use when creating categories.</param>
        public void CreateCategories(IRaster raster)
        {
            GetValues(raster);
            CreateBreakCategories();
            OnItemChanged(this);
        }

        /// <summary>
        /// Creates the category using a random fill color.
        /// </summary>
        /// <param name="fillColor">The base color to use for creating the category.</param>
        /// <param name="size">For points this is the larger dimension, for lines this is the largest width.</param>
        /// <returns>A new IFeatureCategory that matches the type of this scheme.</returns>
        public override ICategory CreateNewCategory(Color fillColor, double size)
        {
            return new ColorCategory(null, null, fillColor, fillColor);
        }

        /// <summary>
        /// Uses the settings on this scheme to create a random category.
        /// </summary>
        /// <returns>A new IFeatureCategory.</returns>
        public override ICategory CreateRandomCategory()
        {
            Random rnd = new(DateTime.Now.Millisecond);
            return CreateNewCategory(CreateRandomColor(rnd), 20);
        }

        /// <summary>
        /// Attempts to decrease the index value of the specified category, and returns
        /// true if the move was successfull.
        /// </summary>
        /// <param name="category">The category to decrease the index of.</param>
        /// <returns>True, if the move was successfull.</returns>
        public override bool DecreaseCategoryIndex(ICategory category)
        {
            return category is IColorCategory cc && _categories.DecreaseIndex(cc);
        }

        /// <summary>
        /// Draws the category in the specified location.
        /// </summary>
        /// <param name="index">Index of the category that gets drawn.</param>
        /// <param name="g">graphics object used for drawing.</param>
        /// <param name="bounds">Rectangle to draw the category to.</param>
        public override void DrawCategory(int index, Graphics g, Rectangle bounds)
        {
            _categories[index].LegendSymbolPainted(g, bounds);
        }

        /// <summary>
        /// Gets the values from the raster. If MaxSampleCount is less than the
        /// number of cells, then it randomly samples the raster with MaxSampleCount
        /// values. Otherwise it gets all the values in the raster.
        /// </summary>
        /// <param name="raster">The raster to sample.</param>
        public void GetValues(IRaster raster)
        {
            Values = raster.GetRandomValues(EditorSettings.MaxSampleCount);
            var keepers = Values.Where(val => val != raster.NoDataValue).ToList();
            Values = keepers;
            Statistics.Calculate(Values);
        }

        /// <summary>
        /// Attempts to increase the position of the specified category, and returns true
        /// if the index increase was successful.
        /// </summary>
        /// <param name="category">The category to increase the position of.</param>
        /// <returns>Boolean, true if the item's position was increased.</returns>
        public override bool IncreaseCategoryIndex(ICategory category)
        {
            return category is IColorCategory cc && _categories.IncreaseIndex(cc);
        }

        /// <summary>
        /// Inserts the item at the specified index.
        /// </summary>
        /// <param name="index">Index where the category gets inserted.</param>
        /// <param name="category">Category that gets inserted.</param>
        public override void InsertCategory(int index, ICategory category)
        {
            if (category is IColorCategory cc) _categories.Insert(index, cc);
        }

        /// <summary>
        /// Removes the specified category.
        /// </summary>
        /// <param name="category">Category that gets removed.</param>
        public override void RemoveCategory(ICategory category)
        {
            if (category is IColorCategory cc) _categories.Remove(cc);
        }

        /// <summary>
        /// Allows the ChangeItem event to get passed on when changes are made.
        /// </summary>
        public override void ResumeEvents()
        {
            _categories.ResumeEvents();
        }

        /// <summary>
        /// Suspends the change item event from firing as the list is being changed.
        /// </summary>
        public override void SuspendEvents()
        {
            _categories.SuspendEvents();
        }

        /// <summary>
        /// Occurs when setting the parent item and updates the parent item pointers.
        /// </summary>
        /// <param name="value">The parent item.</param>
        protected override void OnSetParentItem(ILegendItem value)
        {
            base.OnSetParentItem(value);
            _categories.UpdateItemParentPointers();
        }

        private void Configure()
        {
            _categories = new ColorCategoryCollection(this);
            _opacity = 1;
            EditorSettings = new RasterEditorSettings();
        }

        #endregion
    }
}