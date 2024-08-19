// *********************************************************************************************************
// *
// *  Author:       Maximilian Burkhardt
// *  Disclaimer:   Copyright (c) 2024 - Kauschke Engineering Systems GmbH
// * 				        All rights reserved - Unauthorized deobfuscation and making copies of this file,
// * 				        via any medium is strictly prohibited.
// *
// *********************************************************************************************************

namespace System.Windows.Controls.Automation.Peers
{
    #region

    using System.Collections.Generic;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls;
    using System.Linq;
    using System.Windows.Automation.Peers;
    #endregion

    public class MultiSelectTreeViewExAutomationPeer : ItemsControlAutomationPeer, ISelectionProvider
    {
        #region Constructors and Destructors

        public MultiSelectTreeViewExAutomationPeer(MultiSelectTreeViewEx owner)
            : base(owner)
        {
        }

        #endregion

        #region Public Properties

        public bool CanSelectMultiple
        {
            get
            {
                return true;
            }
        }

        public bool IsSelectionRequired
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Public Methods

        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Selection)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        #endregion

        #region Explicit Interface Methods

        IRawElementProviderSimple[] ISelectionProvider.GetSelection()
        {
            List<IRawElementProviderSimple> list = new List<IRawElementProviderSimple>();

            MultiSelectTreeViewEx treeView = (MultiSelectTreeViewEx)Owner;
            foreach (var item in treeView.GetTreeViewItemsFor(treeView.SelectedItems))
            {
                list.Add(new MultiSelectTreeViewExItemAutomationPeer(item));
            }
         
            return list.ToArray();
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, creates a new instance of the
        /// <see cref="T:System.Windows.Automation.Peers.ItemAutomationPeer"/> for a data item in
        /// the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> collection of this
        /// <see cref="T:System.Windows.Controls.ItemsControl"/>.
        /// </summary>
        /// <param name="item">
        /// The data item that is associated with this <see cref="T:System.Windows.Automation.Peers.ItemAutomationPeer"/>.
        /// </param>
        /// <returns>
        /// The new <see cref="T:System.Windows.Automation.Peers.ItemAutomationPeer"/> created.
        /// </returns>
        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new MultiSelectTreeViewItemDataAutomationPeer(item, this);
        }

        ///// <summary>
        ///// When overridden in a derived class, creates a new instance of the <see cref="T:System.Windows.Automation.Peers.ItemAutomationPeer"/> for a data item in the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> collection of this <see cref="T:System.Windows.Controls.ItemsControl"/>.
        ///// </summary>
        ///// <param name="item">
        ///// The data item that is associated with this <see cref="T:System.Windows.Automation.Peers.ItemAutomationPeer"/>.
        ///// </param>
        ///// <returns>
        ///// The new <see cref="T:System.Windows.Automation.Peers.ItemAutomationPeer"/> created.
        ///// </returns>
        //protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        //{
        //    return null;
        //}

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Tree;
        }

        protected override string GetClassNameCore()
        {
            return "MultiSelectTreeViewEx";
        }

        #endregion
    }
}
