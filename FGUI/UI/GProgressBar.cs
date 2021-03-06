﻿using System;
using FairyGUI.Utils;
using UnityEngine;

namespace FairyGUI
{
    public class GProgressBar : GComponent
    {
        int _max;
        int _value;
        ProgressTitleType _titleType;
        bool _reverse;

        GTextField _titleObject;
        GMovieClip _aniObject;
        GObject _barObjectH;
        GObject _barObjectV;
        float _barMaxWidth;
        float _barMaxHeight;
        float _barMaxWidthDelta;
        float _barMaxHeightDelta;
        float _barStartX;
        float _barStartY;

        public GProgressBar()
        {
            _value = 50;
            _max = 100;
        }

        public ProgressTitleType titleType
        {

            get
            {
                return _titleType;
            }
            set
            {
                if (_titleType != value)
                {
                    _titleType = value;
                    Update();
                }
            }
        }

        public int max
        {
            get
            {
                return _max;
            }
            set
            {
                if (_max != value)
                {
                    _max = value;
                    Update();
                }
            }
        }

        public int value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    Update();
                }
            }
        }


        public void Update()
        {
            float percent = Math.Min((float)_value / _max, 1);
            if (_titleObject != null)
            {
                switch (_titleType)
                {
                    case ProgressTitleType.Percent:
                        _titleObject.text = Mathf.RoundToInt(percent * 100) + "%";
                        break;

                    case ProgressTitleType.ValueAndMax:
                        _titleObject.text = _value + "/" + max;
                        break;

                    case ProgressTitleType.Value:
                        _titleObject.text = "" + _value;
                        break;

                    case ProgressTitleType.Max:
                        _titleObject.text = "" + _max;
                        break;
                }
            }

            float fullWidth = this.width - _barMaxWidthDelta;
            float fullHeight = this.height - _barMaxHeightDelta;
            if (!_reverse)
            {
                if (_barObjectH != null)
                    _barObjectH.width = Mathf.RoundToInt(fullWidth * percent);
                if (_barObjectV != null)
                    _barObjectV.height = Mathf.RoundToInt(fullHeight * percent);
            }
            else
            {
                if (_barObjectH != null)
                {
                    _barObjectH.width = Mathf.RoundToInt(fullWidth * percent);
                    _barObjectH.x = _barStartX + (fullWidth - _barObjectH.width);

                }
                if (_barObjectV != null)
                {
                    _barObjectV.height = Mathf.RoundToInt(fullHeight * percent);
                    _barObjectV.y = _barStartY + (fullHeight - _barObjectV.height);
                }
            }
            if (_aniObject != null)
                _aniObject.frame = Mathf.RoundToInt(percent * 100);
        }

        override public void ConstructFromXML(XML cxml)
        {
            base.ConstructFromXML(cxml);

            XML xml = cxml.GetNode("ProgressBar");

            string str;
            str = xml.GetAttribute("titleType");
            if (str != null)
                _titleType = FieldTypes.ParseProgressTitleType(str);
            else
                _titleType = ProgressTitleType.Percent;
            _reverse = xml.GetAttributeBool("reverse", false);

            _titleObject = GetChild("title") as GTextField;
            _barObjectH = GetChild("bar");
            _barObjectV = GetChild("bar_v");
            _aniObject = GetChild("ani") as GMovieClip;

            if (_barObjectH != null)
            {
                _barMaxWidth = _barObjectH.width;
                _barMaxWidthDelta = this.width - _barMaxWidth;
                _barStartX = _barObjectH.x;
            }
            if (_barObjectV != null)
            {
                _barMaxHeight = _barObjectV.height;
                _barMaxHeightDelta = this.height - _barMaxHeight;
                _barStartY = _barObjectV.y;
            }
            Update();
        }

        override public void Setup_AfterAdd(XML cxml)
        {
            base.Setup_AfterAdd(cxml);

            XML xml = cxml.GetNode("ProgressBar");
            if (xml == null)
                return;

            _value = xml.GetAttributeInt("value");
            _max = xml.GetAttributeInt("max");

            if (!this.underConstruct)
                Update();
        }

        override protected void HandleSizeChanged()
        {
            base.HandleSizeChanged();

            if (_barObjectH != null)
                _barMaxWidth = this.width - _barMaxWidthDelta;
            if (_barObjectV != null)
                _barMaxHeight = this.height - _barMaxHeightDelta;
            Update();
        }
    }
}
