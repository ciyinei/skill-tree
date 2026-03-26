using System;
using UnityEngine;

namespace SkillTree
{
    public interface ICurrencyManager
    {
        float GetCurrency(CurrencyType currencyType);
        bool CanAfford(CurrencyType currencyType, float amount);
        void SpendCurrency(CurrencyType currencyType, float amount);
        void AddCurrency(CurrencyType currencyType, float amount);
        event Action<CurrencyType, float> OnCurrencyChanged;
    }
}
