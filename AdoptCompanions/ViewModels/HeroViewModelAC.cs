using AdoptCompanions.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;

namespace AdoptCompanions.ViewModels
{
    internal class HeroViewModelAC : HeroViewModel
    {
        private readonly Hero hero;
        private readonly string btnTextChild;
        private readonly string btnTextSibling;

        public HeroViewModelAC(Hero hero, CharacterViewModel.StanceTypes stance = 0)
          : base(stance)
        {
            this.hero = hero;
            this.btnTextChild = ((object)new TextObject("{=Adopt_Companion_Adopt}Adopt as Child!", null)).ToString();
            this.btnTextSibling = ((object)new TextObject("{=Adopt_Companion_Adopt}Adopt as Sibling!", null)).ToString();
        }

        public void AdoptHero()
        {
            AdoptionTypeVM adoptionType = new AdoptionTypeVM(AdoptConstants.TYPE_ID_CHILD);

            ACHelper.AdoptAction(this.hero, adoptionType);
            ((ViewModel)this).OnPropertyChanged("IsAlreadyFamily");
        }

        public void AdoptSibling()
        {
            AdoptionTypeVM adoptionType = new AdoptionTypeVM(AdoptConstants.TYPE_ID_SIBLING);

            ACHelper.AdoptAction(this.hero, adoptionType);
            ((ViewModel)this).OnPropertyChanged("IsAlreadyFamily");
        }

        public void AdoptChild()
        {
            AdoptionTypeVM adoptionType = new AdoptionTypeVM(AdoptConstants.TYPE_ID_CHILD);

            ACHelper.AdoptAction(this.hero, adoptionType);
            ((ViewModel)this).OnPropertyChanged("IsAlreadyFamily");
        }

        [DataSourceProperty]
        public string AdoptChildText => this.btnTextChild;

        [DataSourceProperty]
        public string AdoptSiblingText => this.btnTextSibling;


        [DataSourceProperty]
        public bool CanAdopt => (ACHelper.canAdopt(this.hero) > 0) && (ACHelper.checkRelationship(this.hero) == AdoptConstants.PASS_RELATIONSHIP) ? true: false;

        [DataSourceProperty]
        public bool IsAlreadyFamily => ACHelper.isFamily(Hero.MainHero, this.hero) > 0 ? false : true;
    }
}
