using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interfaces;
using Game;

namespace Units
{
    public class GreaterUnit : Unit
    {
        [SerializeField] private UnitAbility[] unitAbilities;
        [SerializeField] private float abilityInterval;

        [Space(10)]
        [SerializeField] private UnitAbility[] onDeathAbilities;

        private UnitAbility selectedAbility;
        private float _interval;

        public override void InitializeEnemy(LevelManager levelManager, int position = 0)
        {
            base.InitializeEnemy(levelManager, position);

            foreach (UnitAbility unitAbility in unitAbilities)
            {
                unitAbility.Initialize(this);
            }

            _interval = abilityInterval;
            initialized = true;
        }

        protected override void Update()
        {
            base.Update();

            if (unitAbilities.Length > 0)
            {
                if (selectedAbility == null && _interval <= 0f)
                {
                    selectedAbility = MathHelper.RandomFromArray(unitAbilities);
                    selectedAbility.EnterState();
                }
                else
                {
                    _interval -= Time.deltaTime;
                }

                if (selectedAbility != null)
                {
                    selectedAbility.UpdateState();
                }
            }
        }

        public void ExitState()
        {
            selectedAbility = null;
            _interval = abilityInterval;
        }

        private void OnDisable()
        {
            if (!initialized)
                return;

            for (int i = 0; i < onDeathAbilities.Length; i++)
            {
                onDeathAbilities[i].Initialize(this);
                onDeathAbilities[i].EnterState();
            }
        }
    }
}