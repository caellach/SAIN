﻿using EFT;
using EFT.InventoryLogic;
using SAIN.Helpers;
using UnityEngine;
using static SAIN.UserSettings.ShootConfig;
using static SAIN.UserSettings.DifficultyConfig;

namespace SAIN.Classes
{
    public class Recoil : SAINWeapon
    {
        public Recoil(BotOwner owner) : base(owner) { }

        public Vector3 CalculateRecoil(Vector3 targetpoint)
        {
            float distance = SAIN.DistanceToAimTarget;

            // Reduces scatter recoil at very close range. Clamps distance between 3 and 20 then scale to 0.25 to 1.
            // So if a target is 3m or less distance, their recoil scaling will be 25% its original value
            distance = Mathf.Clamp(distance, 3f, 20f);
            distance /= 20f;
            distance = distance * 0.75f + 0.25f;

            float weaponhorizrecoil = (CurrentWeapon.Template.RecoilForceUp / RecoilBaseline) * WeaponInfo.FinalModifier;
            float weaponvertrecoil = (CurrentWeapon.Template.RecoilForceBack / RecoilBaseline) * WeaponInfo.FinalModifier;

            float horizRecoil = (1f * weaponhorizrecoil + AddRecoil.Value) * distance;
            float vertRecoil = (1f * weaponvertrecoil + AddRecoil.Value) * distance;

            float maxrecoil = MaxScatter.Value * distance;

            float randomHorizRecoil = Random.Range(-horizRecoil, horizRecoil);
            float randomvertRecoil = Random.Range(-vertRecoil, vertRecoil);

            Vector3 vector = new Vector3(targetpoint.x + randomHorizRecoil, targetpoint.y + randomvertRecoil, targetpoint.z + randomHorizRecoil);
            vector = MathHelpers.VectorClamp(vector, -maxrecoil, maxrecoil) * ConfigModifier;

            return vector;
        }

        public Vector3 CalculateDecay(Vector3 oldVector, out float rate)
        {
            var mode = CurrentWeapon.SelectedFireMode;
            if (mode == Weapon.EFireMode.fullauto || mode == Weapon.EFireMode.burst)
            {
                rate = Time.time + (FullAutoTimePerShot / 3f);
            }
            else
            {
                rate = Time.time + (SemiAutoTimePerShot / 3f);
            }
            return Vector3.Lerp(Vector3.zero, oldVector, LerpRecoil.Value);
        }

        public float RecoilTimeWait
        {
            get
            {
                if (CurrentWeapon.SelectedFireMode == Weapon.EFireMode.fullauto || CurrentWeapon.SelectedFireMode == Weapon.EFireMode.burst)
                {
                    return Time.time + Shoot.FullAutoTimePerShot(CurrentWeapon.Template.bFirerate) * 0.8f;
                }
                else
                {
                    return Time.time + (1f / (CurrentWeapon.Template.SingleFireRate / 60f)) * 0.8f;
                }
            }
        }

        private float ConfigModifier
        {
            get
            {
                float modifier = 1f;
                if (SAIN.Info.IsPMC)
                {
                    modifier *= PMCRecoil.Value;
                }
                else if (SAIN.Info.IsScav)
                {
                    modifier *= ScavRecoil.Value;
                }
                else
                {
                    modifier *= OtherRecoil.Value;
                }
                modifier *= BotRecoilGlobal.Value;
                return modifier;
            }
        }

        private float FullAutoTimePerShot
        {
            get
            {
                float roundspersecond = CurrentWeapon.Template.SingleFireRate / 60;

                float secondsPerShot = 1f / roundspersecond;

                return secondsPerShot;
            }
        }

        private float SemiAutoTimePerShot
        {
            get
            {
                return 1f / (CurrentWeapon.Template.SingleFireRate / 60f);
            }
        }

        private float RecoilBaseline
        {
            get
            {
                if (!SAINPlugin.RealismModLoaded)
                {
                    return 250f;
                }
                else
                {
                    return 112f;
                }
            }
        }
    }
}