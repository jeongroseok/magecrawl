using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using Magecrawl.Interfaces;
using System.Collections.Generic;

namespace MageCrawl.Silverlight.ModelView
{
    public class CharacterModelView : INotifyPropertyChanged
    {
        private IPlayer m_player;

        public event PropertyChangedEventHandler PropertyChanged;

        public CharacterModelView(IPlayer player)
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
