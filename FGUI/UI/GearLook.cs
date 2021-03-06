﻿using System.Collections.Generic;
using DG.Tweening;
using FairyGUI.Utils;
using UnityEngine;

namespace FairyGUI
{
    class GearLookValue
    {
        public float alpha;
        public float rotation;
        public bool grayed;

        public GearLookValue(float alpha, float rotation, bool grayed)
        {
            this.alpha = alpha;
            this.rotation = rotation;
            this.grayed = grayed;
        }
    }

    public class GearLook : GearBase
    {
        Dictionary<string, GearLookValue> _storage;
        GearLookValue _default;
        Tweener _tweener;

        public GearLook(GObject owner)
            : base(owner)
        {
        }

        protected override void Init()
        {
            _default = new GearLookValue(_owner.alpha, _owner.rotation, _owner.grayed);
            _storage = new Dictionary<string, GearLookValue>();
        }

        override protected void AddStatus(string pageId, string value)
        {
            string[] arr = value.Split(jointChar0);
            if (pageId == null)
            {
                _default.alpha = float.Parse(arr[0]);
                _default.rotation = float.Parse(arr[1]);
                _default.grayed = int.Parse(arr[2]) == 1;
            }
            else
                _storage[pageId] = new GearLookValue(float.Parse(arr[0]), float.Parse(arr[1]), int.Parse(arr[2]) == 1);
        }

        override public void Apply()
        {
            GearLookValue gv;
            bool ct = this.connected;
            if (ct)
            {
                if (!_storage.TryGetValue(_controller.selectedPageId, out gv))
                    gv = _default;
            }
            else
                gv = _default;

            if (_tweener != null)
                _tweener.Kill(true);

            if (tween && UIPackage._constructing == 0
                && ct && pageSet.ContainsId(_controller.previousPageId))
            {
                _owner._gearLocked = true;
                _owner.grayed = gv.grayed;
                _owner._gearLocked = false;

                bool a = gv.alpha != _owner.alpha;
                bool b = gv.rotation != _owner.rotation;
                if (a || b)
                {
                    _owner.internalVisible++;
                    _tweener = DOTween.To(() => new Vector2(_owner.alpha, _owner.rotation), val =>
                    {
                        _owner._gearLocked = true;
                        if (a)
                            _owner.alpha = val.x;
                        if (b)
                            _owner.rotation = val.y;
                        _owner._gearLocked = false;
                    }, new Vector2(gv.alpha, gv.rotation), tweenTime)
                    .SetEase(easeType)
                    .OnComplete(() => 
                    { 
                        _tweener = null;
                        _owner.internalVisible--; 
                    });
                }
            }
            else
            {
                _owner._gearLocked = true;
                _owner.alpha = gv.alpha;
                _owner.rotation = gv.rotation;
                _owner.grayed = gv.grayed;
                _owner._gearLocked = false;
            }           
        }

        override public void UpdateState()
        {
            if (_owner._gearLocked)
                return;

            if (connected)
            {
                GearLookValue gv;
                if (!_storage.TryGetValue(_controller.selectedPageId, out gv))
                    _storage[_controller.selectedPageId] = new GearLookValue(_owner.alpha, _owner.rotation, _owner.grayed);
                else
                {
                    gv.alpha = _owner.alpha;
                    gv.rotation = _owner.rotation;
                    gv.grayed = _owner.grayed;
                }
            }
            else
            {
                _default.alpha = _owner.alpha;
                _default.rotation = _owner.rotation;
                _default.grayed = _owner.grayed;
            }
        }
    }
}
