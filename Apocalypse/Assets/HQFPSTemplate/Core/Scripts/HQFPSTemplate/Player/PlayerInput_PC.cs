using UnityEngine;

namespace HQFPSTemplate
{
	/// <summary>
	/// Simple component that handles the player input, and feeds it to the other components.
	/// </summary>
	public class PlayerInput_PC : PlayerComponent
	{
		private void Update()
		{
			if (!Player.Pause.Active && Player.ViewLocked.Is(false))
			{
				// Movement.
				Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

				//Little Hack until a cleaner solution will be found
				if (Player.Aim.Active && Player.Prone.Active)
					moveInput = Vector2.zero;

				Player.MoveInput.Set(moveInput);

				// Look.
				Player.LookInput.Set(new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")));

				// Interact
				Player.Interact.Set(Input.GetButton("Interact"));

				// Jump.
				if (Input.GetButtonDown("Jump"))
					Player.Jump.TryStart();

				// Run.
				bool sprintButtonHeld = Input.GetButton("Run");
				bool canStartSprinting = Player.IsGrounded.Get() && Player.MoveInput.Get().y > 0f;

				if (!Player.Run.Active && sprintButtonHeld && canStartSprinting)
					Player.Run.TryStart();

				if (Player.Run.Active && !sprintButtonHeld)
					Player.Run.ForceStop();

				if (Input.GetButtonDown("Crouch"))
				{
					if (!Player.Crouch.Active)
						Player.Crouch.TryStart();
					else
						Player.Crouch.TryStop();
				}
				else if (Input.GetButtonDown("Prone"))
				{
					if (!Player.Prone.Active)
						Player.Prone.TryStart();
					else
						Player.Prone.TryStop();
				}

				UseEquipment();

				//Suicide (Used for testing)
				if (Input.GetKeyDown(KeyCode.K))
				{
					DamageInfo damage = new DamageInfo(-1000f);
					Player.ChangeHealth.Try(damage);
				}
			}
			else
			{
				// Movement.
				Player.MoveInput.Set(Vector2Int.zero);

				// Look.
				Player.LookInput.Set(Vector2.zero);
			}

			var scrollWheelValue = Input.GetAxisRaw("Mouse ScrollWheel");
			Player.ScrollValue.Set(scrollWheelValue);
		}	

		private void UseEquipment()
		{
			if (Input.GetButtonDown("Drop") && !Player.EquippedItem.Is(null) && !Player.Reload.Active && !Player.Healing.Active)
			{
				Player.DropItem.Try(Player.EquippedItem.Get());
				return;
			}

			// Change use mode
			if (Input.GetButtonDown("ChangeUseMode"))
				Player.ChangeUseMode.Try();

			bool alternateUseButtonHeld = Input.GetButton("AlternateUse");

			// Use item
			if (Input.GetButtonDown("UseEquipment"))
				Player.UseItem.Try(false, alternateUseButtonHeld ? 1 : 0);
			else if (Input.GetButton("UseEquipment"))
				Player.UseItem.Try(true, alternateUseButtonHeld ? 1 : 0);

			if (Input.GetButtonDown("ReloadEquipment"))
				Player.Reload.TryStart();

			// Aim
			var aimButtonPressed = Input.GetButton("Aim");

			if (!Player.Aim.Active && aimButtonPressed)
				Player.Aim.TryStart();
			else if (Player.Aim.Active && !aimButtonPressed)
				Player.Aim.ForceStop();

			//Heal
			if (Input.GetButton("Heal") && !aimButtonPressed)
				Player.Healing.TryStart();
		}
	}
}
