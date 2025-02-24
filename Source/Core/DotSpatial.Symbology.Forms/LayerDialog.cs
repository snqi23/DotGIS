// Copyright (c) DotSpatial Team. All rights reserved.
// Licensed under the MIT license. See License.txt file in the project root for full license information.

using System;
using System.Data;
using System.Windows.Forms;

namespace DotSpatial.Symbology.Forms
{
    /// <summary>
    /// This is a basic form which is displayed when the user double-clicks on a layer name
    /// in the legend.
    /// </summary>
    public partial class LayerDialog : Form
    {
        #region Fields

        private readonly ILayer _layer;
        private ICategoryControl _rasterCategoryControl;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerDialog"/> class.
        /// </summary>
        public LayerDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerDialog"/> class to display the symbology and
        /// other properties of the specified feature layer.
        /// </summary>
        /// <param name="selectedLayer">the specified feature layer that is
        /// modified using this form.</param>
        /// <param name="control">The control.</param>
        public LayerDialog(ILayer selectedLayer, ICategoryControl control)
            : this()
        {
            _layer = selectedLayer;
            propertyGrid1.SelectedObject = _layer;
            Configure(control);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the apply changes situation forces the symbology to become updated.
        /// </summary>
        public event EventHandler ChangesApplied;

        #endregion

        #region Methods

        /// <summary>
        /// Forces changes to be written from the copy symbology to
        /// the original, updating the map display.
        /// </summary>
        public void ApplyChanges()
        {
            try
            {
                OnApplyChanges();
            }
            catch (SyntaxErrorException)
            {
                MessageBox.Show(SymbologyFormsMessageStrings.LayerDialog_InvalidExpressionProvided);
            }
        }

        /// <summary>
        /// Occurs during apply changes operations and is overrideable in subclasses.
        /// </summary>
        protected virtual void OnApplyChanges()
        {
            _rasterCategoryControl.ApplyChanges();

            ChangesApplied?.Invoke(_layer, EventArgs.Empty);
        }

        private void Configure(ICategoryControl control)
        {
            if (control is UserControl userControl)
            {
                userControl.Parent = pnlContent;
                userControl.Visible = true;
            }

            _rasterCategoryControl = control;
            _rasterCategoryControl.Initialize(_layer);
        }

        private void DialogButtons1ApplyClicked(object sender, EventArgs e)
        {
            OnApplyChanges();
        }

        private void DialogButtons1CancelClicked(object sender, EventArgs e)
        {
            _rasterCategoryControl.Cancel();
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void DialogButtons1OkClicked(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            OnApplyChanges();
            Close();
        }

        #endregion
    }
}