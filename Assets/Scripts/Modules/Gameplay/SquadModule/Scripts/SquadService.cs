using System;
using System.Collections.Generic;
using UniRx;

namespace vikwhite
{
    public interface ISquadService
    {
        IReadOnlyList<Character> SelectedCharacters { get; }
        IReadOnlyReactiveProperty<int> SelectedCount { get; }
        IReadOnlyReactiveProperty<int> PlayerMight { get; }
        IReadOnlyReactiveProperty<int> EnemyMight { get; }

        event Action<Character, int> CharacterSelected;
        event Action<Character, int> CharacterDeselected;
        event Action FightRequested;
        event Action BackRequested;

        bool IsSelected(Character character);
        bool TrySelect(Character character);
        void Deselect(Character character);
        void Clear();
        void SetEnemyMight(int might);
        void RequestFight();
        void RequestBack();
    }

    public class SquadService : ISquadService
    {
        public const int MaxCharacters = 5;

        private readonly Character[] _selectedCharacters = new Character[MaxCharacters];
        private readonly ReactiveProperty<int> _selectedCount = new();
        private readonly ReactiveProperty<int> _playerMight = new();
        private readonly ReactiveProperty<int> _enemyMight = new();

        public IReadOnlyList<Character> SelectedCharacters => _selectedCharacters;
        public IReadOnlyReactiveProperty<int> SelectedCount => _selectedCount;
        public IReadOnlyReactiveProperty<int> PlayerMight => _playerMight;
        public IReadOnlyReactiveProperty<int> EnemyMight => _enemyMight;

        public event Action<Character, int> CharacterSelected;
        public event Action<Character, int> CharacterDeselected;
        public event Action FightRequested;
        public event Action BackRequested;

        public bool IsSelected(Character character)
        {
            return character != null && Array.IndexOf(_selectedCharacters, character) >= 0;
        }

        public bool TrySelect(Character character)
        {
            if (character == null) return false;
            if (IsSelected(character)) return true;

            var slot = Array.IndexOf(_selectedCharacters, null);
            if (slot < 0) return false;

            _selectedCharacters[slot] = character;
            _selectedCount.Value++;
            RecalculatePlayerMight();
            CharacterSelected?.Invoke(character, slot);
            return true;
        }

        public void Deselect(Character character)
        {
            if (character == null) return;

            var slot = Array.IndexOf(_selectedCharacters, character);
            if (slot < 0) return;

            _selectedCharacters[slot] = null;
            _selectedCount.Value--;
            RecalculatePlayerMight();
            CharacterDeselected?.Invoke(character, slot);
        }

        public void Clear()
        {
            Array.Clear(_selectedCharacters, 0, _selectedCharacters.Length);
            _selectedCount.Value = 0;
            _playerMight.Value = 0;
            _enemyMight.Value = 0;
        }

        public void SetEnemyMight(int might)
        {
            _enemyMight.Value = Math.Max(0, might);
        }

        public void RequestFight()
        {
            if (_selectedCount.Value > 0)
                FightRequested?.Invoke();
        }

        public void RequestBack()
        {
            BackRequested?.Invoke();
        }

        private void RecalculatePlayerMight()
        {
            var might = 0;
            foreach (var character in _selectedCharacters)
            {
                if (character != null)
                    might += character.Might.Value;
            }
            _playerMight.Value = might;
        }
    }
}
