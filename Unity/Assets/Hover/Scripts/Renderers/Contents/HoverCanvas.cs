﻿using System;
using Hover.Renderers.Utils;
using Hover.Utils;
using UnityEngine;

namespace Hover.Renderers.Contents {

	/*================================================================================================*/
	[ExecuteInEditMode]
	[RequireComponent(typeof(TreeUpdater))]
	[RequireComponent(typeof(Canvas))]
	[RequireComponent(typeof(CanvasGroup))]
	public class HoverCanvas : MonoBehaviour, ISettingsController, ITreeUpdateable {
		
		//TODO: handle alpha-related settings elsewhere
		//TODO: use "shape" reference here? (instead of "HoverCanvasSizeUpdater")

		public const string SizeXName = "SizeX";
		public const string SizeYName = "SizeY";
		public const string AlphaName = "Alpha";
		public const string RenderQueueName = "RenderQueue";

		public enum CanvasAlignmentType {
			Left,
			Center,
			Right,
			TextLeftAndIconRight,
			TextRightAndIconLeft,
			Custom
		}
		
		public enum IconSizeType {
			FontSize,
			ThreeQuartersFontSize,
			OneAndHalfFontSize,
			DoubleFontSize,
			Custom
		}
		
		public ISettingsControllerMap Controllers { get; private set; }
		
		[DisableWhenControlled(DisplayMessage=true)]
		public HoverLabel Label;
		public HoverIcon IconOuter;
		public HoverIcon IconInner;
		
		[DisableWhenControlled(RangeMin=0.0001f)]
		public float Scale = 0.0002f;
		
		[DisableWhenControlled(RangeMin=0)]
		public float SizeX = 0.1f;

		[DisableWhenControlled(RangeMin=0)]
		public float SizeY = 0.1f;
		
		[DisableWhenControlled(RangeMin=0)]
		public float PaddingX = 0.005f;
		
		[DisableWhenControlled(RangeMin=0)]
		public float PaddingY = 0.005f;

		[DisableWhenControlled(RangeMin=0, RangeMax=1)]
		public float Alpha = 1; //TODO: remove this
		
		[DisableWhenControlled]
		public CanvasAlignmentType Alignment = CanvasAlignmentType.Left;

		[DisableWhenControlled]
		public IconSizeType IconSize = IconSizeType.FontSize;

		[HideInInspector]
		[SerializeField]
		private bool _IsBuilt;
		
		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public HoverCanvas() {
			Controllers = new SettingsControllerMap();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public Canvas CanvasComponent {
			get { return GetComponent<Canvas>(); }
		}

		/*--------------------------------------------------------------------------------------------*/
		public CanvasGroup CanvasGroupComponent {
			get { return GetComponent<CanvasGroup>(); }
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public float UnscaledPaddedSizeX {
			get { return SizeX-PaddingX*2; }
		}

		/*--------------------------------------------------------------------------------------------*/
		public float UnscaledPaddedSizeY {
			get { return SizeY-PaddingY*2; }
		}
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void Awake() {
			if ( !_IsBuilt ) {
				BuildElements();
				_IsBuilt = true;
			}
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public virtual void Start() {
			//do nothing...
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public void TreeUpdate() {
			UpdateCanvasComponent();
			UpdateGeneralSettings();
			UpdateActiveStates();
			UpdateIconSizeSettings();
			UpdateCanvasAlignmentSettings();
			Controllers.TryExpireControllers();
		}
		
		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void BuildElements() {
			CanvasComponent.renderMode = RenderMode.WorldSpace;
			CanvasComponent.sortingOrder = 1;
			
			Label = BuildLabel();
			IconOuter = BuildIcon("IconOuter");
			IconInner = BuildIcon("IconInner");
			
			IconOuter.IconType = HoverIcon.IconOffset.RadioOuter;
			IconInner.IconType = HoverIcon.IconOffset.RadioInner;

			IconInner.ImageComponent.color = new Color(1, 1, 1, 0.7f);
		}
		
		/*--------------------------------------------------------------------------------------------*/
		private HoverLabel BuildLabel() {
			var labelGo = new GameObject("Label");
			labelGo.transform.SetParent(gameObject.transform, false);
			return labelGo.AddComponent<HoverLabel>();
		}
		
		/*--------------------------------------------------------------------------------------------*/
		private HoverIcon BuildIcon(string pName) {
			var iconGo = new GameObject(pName);
			iconGo.transform.SetParent(gameObject.transform, false);
			return iconGo.AddComponent<HoverIcon>();
		}
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void UpdateCanvasComponent() {
			Canvas canvas = CanvasComponent;
			RectTransform rectTx = canvas.GetComponent<RectTransform>();

			gameObject.transform.localScale = Vector3.one*Scale;
			
			rectTx.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, UnscaledPaddedSizeX/Scale);
			rectTx.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, UnscaledPaddedSizeY/Scale);
		}
		
		/*--------------------------------------------------------------------------------------------*/
		private void UpdateGeneralSettings() {
			Label.Controllers.Set("GameObject.activeSelf", this);
			Label.Controllers.Set(HoverLabel.CanvasScaleName, this);
			Label.Controllers.Set(HoverLabel.SizeXName, this);
			Label.Controllers.Set(HoverLabel.SizeYName, this);
			Label.Controllers.Set("Text.alignment", this);
			Label.Controllers.Set("Text.material.renderQueue", this);
			
			IconOuter.Controllers.Set("GameObject.activeSelf", this);
			IconOuter.Controllers.Set(HoverIcon.CanvasScaleName, this);
			IconOuter.Controllers.Set(HoverIcon.SizeXName, this);
			IconOuter.Controllers.Set(HoverIcon.SizeYName, this);
			
			IconInner.Controllers.Set("GameObject.activeSelf", this);
			IconInner.Controllers.Set(HoverIcon.CanvasScaleName, this);
			IconInner.Controllers.Set(HoverIcon.SizeXName, this);
			IconInner.Controllers.Set(HoverIcon.SizeYName, this);
			
			Label.CanvasScale = Scale;
			IconOuter.CanvasScale = Scale;
			IconInner.CanvasScale = Scale;
		}
		
		/*--------------------------------------------------------------------------------------------*/
		private void UpdateActiveStates() {
			bool isLabelActive = (!string.IsNullOrEmpty(Label.TextComponent.text));
			bool isIconOuterActive = (IconOuter.IconType != HoverIcon.IconOffset.None);
			bool isIconInnerActive = (IconInner.IconType != HoverIcon.IconOffset.None);

			RendererUtil.SetActiveWithUpdate(Label, isLabelActive);
			RendererUtil.SetActiveWithUpdate(IconOuter, isIconOuterActive);
			RendererUtil.SetActiveWithUpdate(IconInner, isIconInnerActive);
		}
				
		/*--------------------------------------------------------------------------------------------*/
		private void UpdateIconSizeSettings() {
			if ( IconSize == IconSizeType.Custom ) {
				return;
			}
			
			float fontSize = Label.TextComponent.fontSize*Label.CanvasScale;
			
			switch ( IconSize ) {
				case IconSizeType.FontSize:
					IconOuter.SizeX = fontSize;
					break;
					
				case IconSizeType.ThreeQuartersFontSize:
					IconOuter.SizeX = fontSize*0.75f;
					break;
					
				case IconSizeType.OneAndHalfFontSize:
					IconOuter.SizeX = fontSize*1.5f;
					break;
					
				case IconSizeType.DoubleFontSize:
					IconOuter.SizeX = fontSize*2;
					break;
			}
			
			IconOuter.SizeY = IconOuter.SizeX;
			IconInner.SizeX = IconOuter.SizeX;
			IconInner.SizeY = IconOuter.SizeY;
		}
		
		/*--------------------------------------------------------------------------------------------*/
		private void UpdateCanvasAlignmentSettings() {
			if ( Alignment == CanvasAlignmentType.Custom ) {
				return;
			}
			
			const float iconVertShiftMult = -0.35f;
			
			float fontSize = Label.TextComponent.fontSize*Label.CanvasScale/2;
			float iconAvailW = UnscaledPaddedSizeX-IconOuter.SizeX;
			float iconPad = IconOuter.SizeX*0.2f;
			float iconShiftX = 0;
			float iconShiftY = 0;
			float labelInsetL = 0;
			float labelInsetR = 0;
			float labelInsetT = 0;
			TextAnchor labelAlign;
			
			switch ( Alignment ) { //icon
				case CanvasAlignmentType.Left:
				case CanvasAlignmentType.TextRightAndIconLeft:
					iconShiftX = -0.5f*iconAvailW;
					iconShiftY = iconVertShiftMult*fontSize;
					labelInsetL = IconOuter.SizeX+iconPad;
					break;
					
				case CanvasAlignmentType.Center:
					iconShiftY = (fontSize+iconPad)/2;
					labelInsetT = (IconOuter.SizeY+iconPad)/2;
					break;
					
				case CanvasAlignmentType.Right:
				case CanvasAlignmentType.TextLeftAndIconRight:
					iconShiftX = 0.5f*iconAvailW;
					iconShiftY = iconVertShiftMult*fontSize;
					labelInsetR = IconOuter.SizeX+iconPad;
					break;
					
				default:
					throw new Exception("Unhandled alignment: "+Alignment);
			}
			
			switch ( Alignment ) { //label
				case CanvasAlignmentType.Left:
				case CanvasAlignmentType.TextLeftAndIconRight:
					labelAlign = TextAnchor.MiddleLeft;
					break;
					
				case CanvasAlignmentType.Center:
					labelAlign = TextAnchor.MiddleCenter;
					break;
					
				case CanvasAlignmentType.Right:
				case CanvasAlignmentType.TextRightAndIconLeft:
					labelAlign = TextAnchor.MiddleRight;
					break;
					
				default:
					throw new Exception("Unhandled alignment: "+Alignment);
			}
			
			if ( !IconOuter.gameObject.activeSelf && !IconInner.gameObject.activeSelf ) {
				iconShiftX = 0;
				iconShiftY = 0;
				labelInsetL = 0;
				labelInsetR = 0;
				labelInsetT = 0;
			}
			
			var labelLocalPos = new Vector3((labelInsetL-labelInsetR)/2, -labelInsetT, 0);
			var iconLocalPos = new Vector3(iconShiftX, iconShiftY, 0);
			
			Label.SizeX = UnscaledPaddedSizeX-labelInsetL-labelInsetR;
			Label.SizeY = UnscaledPaddedSizeY-labelInsetT;
			
			Label.TextComponent.alignment = labelAlign;
			
			Label.transform.localPosition = labelLocalPos/Scale;
			IconOuter.transform.localPosition = iconLocalPos/Scale;
			IconInner.transform.localPosition = IconOuter.transform.localPosition;
		}

	}

}
