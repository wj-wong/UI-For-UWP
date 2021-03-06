﻿using System.Collections.Generic;
using System.Linq;
using Telerik.Geospatial;
using Telerik.UI.Xaml.Controls.Map;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;

namespace Telerik.UI.Automation.Peers
{
    /// <summary>
    /// AutomationPeer class for <see cref="MapShapeLayer"/>.
    /// </summary>
    public class MapShapeLayerAutomationPeer : RadControlAutomationPeer, ISelectionProvider
    {
        private Dictionary<string, MapShapeAutomationPeer> childrenCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="MapShapeLayerAutomationPeer"/> class.
        /// </summary>
        public MapShapeLayerAutomationPeer(MapShapeLayer owner) : base(owner)
        {
            this.LayerOwner = owner;
        }

        /// <summary>
        /// Gets a value indicating whether multiple selection is possible in the layer.
        /// </summary>
        public bool CanSelectMultiple
        {
            get
            {
                if (this.SelectionBehavior != null)
                {
                    return this.SelectionBehavior.SelectionMode == MapShapeSelectionMode.MultiSimple ||
                           this.SelectionBehavior.SelectionMode == MapShapeSelectionMode.MultiExtended;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether selection is required in the layer.
        /// </summary>
        public bool IsSelectionRequired
        {
            get
            {
                return false;
            }
        }

        internal MapShapeLayer LayerOwner
        {
            get;
            set;
        }

        internal MapShapeSelectionBehavior SelectionBehavior
        {
            get
            {
                foreach (MapBehavior behavior in this.LayerOwner.Owner.Behaviors)
                {
                    if (behavior is MapShapeSelectionBehavior)
                    {
                        return (MapShapeSelectionBehavior)behavior;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// List of the selected peers' providers.
        /// </summary>
        public IRawElementProviderSimple[] GetSelection()
        {
            List<IRawElementProviderSimple> samples = new List<IRawElementProviderSimple>();
            foreach (MapShapeAutomationPeer peer in this.childrenCache.Values)
            {
                if (peer.IsSelected == true)
                {
                    samples.Add(this.ProviderFromPeer(peer));
                }
            }
            return samples.ToArray();
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "map shape layer";
        }
        
        /// <summary>
        /// Gets the list of child peers.
        /// Currently runtime changes of shapes is not supported.
        /// For example removing 3 shapes re-adding 3 new shapes will not recreate the child peers.
        /// </summary>
        protected override IList<AutomationPeer> GetChildrenCore()
        {
            if (this.childrenCache == null || this.childrenCache.Count != this.LayerOwner.ShapeModels.Count)
            {
                this.childrenCache = new Dictionary<string, MapShapeAutomationPeer>();

                foreach (MapShapeModel model in this.LayerOwner.ShapeModels.OfType<MapShapeModel>())
                {
                    MapShapeAutomationPeer peer = new MapShapeAutomationPeer(this, model);
                    this.childrenCache[model.GetHashCode().ToString()] = peer;
                }
            }
            return this.childrenCache.Values.ToList<AutomationPeer>();
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.List;
        }

        /// <inheritdoc />
        protected override object GetPatternCore(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Selection)
            {
                return this;
            }
            return base.GetPatternCore(patternInterface);
        }
    }
}
