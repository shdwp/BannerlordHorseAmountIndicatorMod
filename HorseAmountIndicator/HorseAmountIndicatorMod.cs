using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using UIExtenderLib;
using UIExtenderLib.Prefab;
using UIExtenderLib.ViewModel;

namespace HorseAmountIndicator
{
    [PrefabExtension("MapBar", "descendant::ListPanel[@Id='BottomInfoBar']/Children")]
    public class BottomBarExtension : PrefabExtensionInsertPatch
    {
        public override int Position => PositionLast;
        public override string Prefab => "HorseAmountIndicator";
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
        
        public override void Refresh()
        {
            var horses = MobileParty.MainParty.ItemRoster.Where(i => i.EquipmentElement.Item.ItemCategory.Id == new MBGUID(671088673));
            var newTooltip = horses.Aggregate("Horses: ", (s, element) => $"{s}\n{element.EquipmentElement.Item.Name}: {element.Amount}");

            if (newTooltip != _horsesTooltip)
            {
                _horsesAmount = horses.Sum(item => item.Amount);
                _horsesTooltip = newTooltip;

                _vm.OnPropertyChanged(nameof(HorsesAmount));
            }
        }
    }
    
    public class HorseAmountIndicatorMod : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            
            UIExtender.Register();
        }
    }
}
