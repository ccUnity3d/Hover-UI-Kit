using Hover.Common.Items.Types;
using UnityEditor;

namespace Hover.Common.Edit.Items.Types {

	/*================================================================================================*/
	[CustomEditor(typeof(HoverCheckboxItem))]
	public class HoverCheckboxItemEditor : HoverSelectableItemBoolEditor {

		
		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public override void OnEnable() {
			base.OnEnable();
			vValueLabel = "Checkbox Value";
		}
		
	}

}