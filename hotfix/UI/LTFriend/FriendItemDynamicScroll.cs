namespace Hotfix_LT.UI
{
    public class FriendItemDynamicScroll : DynamicGridScroll<FriendData, FriendItem>
    {
        // public void ChangeSize(Vector4 _clipoffset)
        // {
        //     mPanel.clipOffset = Vector2.zero;
        //    
        //     mPanel.baseClipRegion = _clipoffset;
        //     mPanelSize = mPanel.GetViewSize();
        //     mPanelOrigionClipOffset = mPanel.clipOffset;
        //     mLastScrollOffset = mPanelOrigionClipOffset;
        //
        //     mDirty = true;
        // }

        public override void Awake()
        {
            base.Awake();
            
        }
    }
}