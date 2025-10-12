using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server._NC.Doors.Components
{
    /// <summary>
    /// Компонент персональной двери: хранит владельца, состояние блокировки и данные для назначения владельца.
    /// </summary>
    [RegisterComponent]
    public sealed partial class PersonalDoorComponent : Component  // ← Добавлено partial
    {
        /// <summary>
        /// Владелец двери (UID игрока). null, если ещё не назначен.
        /// </summary>
        [DataField("owner")]
        public EntityUid? OwnerUid = null;

        /// <summary>
        /// Блокировка двери. По умолчанию true (дверь заблокирована до разблокировки владельцем).
        /// </summary>
        [DataField("isLocked")]
        public bool IsLocked = true;

        /// <summary>
        /// Счётчик последовательных кликов владельца для назначения (нужно два подряд).
        /// </summary>
        [DataField("ownershipClicks")]
        public int OwnershipClicks = 0;

        /// <summary>
        /// Последний кликнувший пользователь — для отслеживания, что клики подряд делает один и тот же игрок.
        /// </summary>
        [DataField("lastClickUser")]
        public EntityUid? LastClickUser = null;
    }
}
