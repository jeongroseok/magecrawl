using System;
using System.Collections.Generic;
using System.ComponentModel;
using Magecrawl.Interfaces;

namespace MageCrawl.Silverlight.ModelView
{
    public class CharacterViewModel : INotifyPropertyChanged
    {
        private IPlayer m_player;

        public event PropertyChangedEventHandler PropertyChanged;

        public CharacterViewModel(IPlayer player)
        {
            m_player = player;
        }

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public void NewTurn()
        {
            NotifyPropertyChanged("Name");
        }

        public string Name
        {
            get
            {
                return m_player.Name;
            }
        }

        public int CurrentStamina
        {
            get
            {
                return m_player.CurrentStamina;
            }
        }

        public int CurrentHealth
        {
            get
            {
                return m_player.CurrentHealth;
            }
        }

        public int MaxStamina
        {
            get
            {
                return m_player.MaxStamina;
            }
        }

        public int MaxHealth
        {
            get
            {
                return m_player.MaxHealth;
            }
        }

        public string HealthString
        {
            get
            {
                return string.Format("Hlth {0}/{1}  Sta {2}/{3}", CurrentHealth, MaxHealth, CurrentStamina, MaxStamina);
            }
        }

        public string ManaString
        {
            get
            {
                return string.Format("Mana {0}/{1}({2})", CurrentMP, MaxMP, MaxPossibleMP);
            }
        }

        public int CurrentMP
        {
            get
            {
                return m_player.CurrentMP;
            }
        }

        public int MaxMP
        {
            get
            {
                return m_player.MaxMP;
            }
        }

        public int MaxPossibleMP
        {
            get
            {
                return m_player.MaxPossibleMP;
            }
        }

        public int SkillPoints
        {
            get
            {
                return m_player.SkillPoints;
            }
        }

        public IEnumerable<IStatusEffect> StatusEffects
        {
            get
            {
                return m_player.StatusEffects;
            }
        }

        public IWeapon CurrentWeapon
        {
            get
            {
                return m_player.CurrentWeapon;
            }
        }
    }
}
