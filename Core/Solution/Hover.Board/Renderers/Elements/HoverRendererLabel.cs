﻿using UnityEngine;
using UnityEngine.UI;

namespace Hover.Board.Renderers.Elements {

	/*================================================================================================*/
	[ExecuteInEditMode]
	[RequireComponent(typeof(Text))]
	public class HoverRendererLabel : MonoBehaviour {

		public bool ControlledByRenderer { get; set; }
		
		[Range(0.01f, 1)]
		public float CanvasScale = 0.02f;
		
		[Range(0, 100)]
		public float SizeX = 10;

		[Range(0, 100)]
		public float SizeY = 10;
		
		[Range(0, 20)]
		public float PaddingX = 0.5f;
		
		[Range(0, 50)]
		public float InsetL = 0;
		
		[Range(0, 50)]
		public float InsetR = 0;

		[HideInInspector]
		[SerializeField]
		private bool vIsBuilt;
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public Text TextComponent {
			get { return GetComponent<Text>(); }
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public void Awake() {
			if ( !vIsBuilt ) {
				BuildText();
				vIsBuilt = true;
			}
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public void Update() {
			if ( !ControlledByRenderer ) {
				UpdateAfterRenderer();
			}
		}
		
		/*--------------------------------------------------------------------------------------------*/
		public void UpdateAfterRenderer() {
			float textX = (PaddingX+InsetL)/CanvasScale;
			float textSizeX = (SizeX-PaddingX*2-InsetL-InsetR)/CanvasScale;
			float textSizeY = SizeY/CanvasScale;
			RectTransform rectTx = TextComponent.rectTransform;

			rectTx.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, textX, textSizeX);
			rectTx.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, textSizeY);
		}
		

		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		private void BuildText() {
			Text text = TextComponent;
			text.text = "Label";
			text.font = Resources.Load<Font>("Fonts/Tahoma");
			text.fontSize = 40;
			text.lineSpacing = 0.75f;
			text.color = Color.white;
			text.alignment = TextAnchor.MiddleCenter;
			text.raycastTarget = false;
		}

	}

}
