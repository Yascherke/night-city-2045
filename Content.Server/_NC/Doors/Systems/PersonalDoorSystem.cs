using Content.Server._NC.Doors.Components;
using Content.Shared.Access.Components;
using Content.Shared.Doors;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;

namespace Content.Server._NC.Doors.Systems
{
    public sealed class PersonalDoorSystem : EntitySystem
    {
        [Dependency] private readonly SharedPopupSystem _popup = default!;

        public override void Initialize()
        {
            // Обработка использования ID-карты по двери
            SubscribeLocalEvent<PersonalDoorComponent, InteractUsingEvent>(OnInteractUsing);

            // Блокировать открытие заблокированной двери
            SubscribeLocalEvent<PersonalDoorComponent, BeforeDoorOpenedEvent>(OnBeforeDoorOpened);
        }

        private void OnInteractUsing(EntityUid uid, PersonalDoorComponent component, InteractUsingEvent args)
        {
            // Проверяем, что используется ID-карта
            if (!TryComp<IdCardComponent>(args.Used, out var idCard))
                return;

            var user = args.User;

            // Если владельца нет - обрабатываем назначение владельца
            if (component.OwnerUid == null)
            {
                HandleOwnershipAssignment(uid, component, user);
            }
            else
            {
                // Владелец есть - обрабатываем блокировку/разблокировку
                HandleLockToggle(uid, component, user);
            }

            args.Handled = true;
        }

        private void HandleOwnershipAssignment(EntityUid doorUid, PersonalDoorComponent component, EntityUid user)
        {
            // Если это другой пользователь - сбрасываем счётчик
            if (component.LastClickUser != user)
            {
                component.OwnershipClicks = 1;
                component.LastClickUser = user;
                _popup.PopupEntity("Первый клик зарегистрирован. Кликните ещё раз для подтверждения владения.", doorUid, user);
                return;
            }

            // Тот же пользователь - увеличиваем счётчик
            component.OwnershipClicks++;

            if (component.OwnershipClicks >= 2)
            {
                // Назначаем владельца
                component.OwnerUid = user;
                component.IsLocked = true; // По умолчанию заблокирована
                _popup.PopupEntity("Вы стали владельцем этой двери!", doorUid, user);

                // Сбрасываем временные данные
                component.OwnershipClicks = 0;
                component.LastClickUser = null;
            }
            else
            {
                _popup.PopupEntity("Требуется второй клик для подтверждения владения.", doorUid, user);
            }
        }

        private void HandleLockToggle(EntityUid doorUid, PersonalDoorComponent component, EntityUid user)
        {
            // Проверяем, что пользователь - владелец
            if (component.OwnerUid != user)
            {
                _popup.PopupEntity("Отсутствует доступ.", doorUid, user);
                return;
            }

            // Переключаем блокировку
            component.IsLocked = !component.IsLocked;
            var message = component.IsLocked ? "Дверь заблокирована." : "Дверь разблокирована.";
            _popup.PopupEntity(message, doorUid, user);
        }

        private void OnBeforeDoorOpened(EntityUid uid, PersonalDoorComponent component, ref BeforeDoorOpenedEvent args)
        {
            // Если дверь заблокирована и есть владелец - блокируем открытие
            if (component.IsLocked && component.OwnerUid != null)
            {
                args.Cancel();

                if (args.User != null)
                {
                    _popup.PopupEntity("Дверь заблокирована.", uid, args.User.Value);
                }
            }
        }
    }
}
