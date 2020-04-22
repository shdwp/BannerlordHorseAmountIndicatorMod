using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using UIExtenderLib;
using UIExtenderLib.Interface;

namespace HorseAmountIndicator
{
    [PrefabExtension("MapBar", "descendant::ListPanel[@Id='BottomInfoBar']/Children")]
    public class BottomBarExtension : PrefabExtensionInsertPatch
    {
        public override int Position => PositionLast;
        public override string Name => "HorseAmountIndicator";
    }

    [ViewModelMixin]
    public class MapInfoMixin : BaseViewModelMixin<MapInfoVM>
    {
        private int _horsesAmount;
        private string _horsesTooltip;
        
        [DataSourceProperty] public BasicTooltipViewModel HorsesAmountHint => new BasicTooltipViewModel(() => _horsesTooltip);
        [DataSourceProperty] public string HorsesAmount => "" + _horsesAmount;

        public MapInfoMixin(MapInfoVM vm) : base(vm)
        {
        }
        
        public override void OnRefresh()
        {
            var horses = MobileParty.MainParty.ItemRoster.Where(i => i.EquipmentElement.Item.ItemCategory.Id == new MBGUID(671088673));
            var newTooltip = horses.Aggregate("Horses: ", (s, element) => $"{s}\n{element.EquipmentElement.Item.Name}: {element.Amount}");

            if (newTooltip != _horsesTooltip)
            {
                _horsesAmount = horses.Sum(item => item.Amount);
                _horsesTooltip = newTooltip;

                if (_vm.TryGetTarget(out var vm))
                {
                    vm.OnPropertyChanged(nameof(HorsesAmount));
                }
            }
        }
    }
    
    public class HorseAmountIndicatorMod : MBSubModuleBase
    {
        private UIExtender _extender;
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            
            _extender = new UIExtender("HorseAmountIndicatorMod");
            _extender.Register();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            _extender.Verify();
        }
    }
}
