using Ore2Lib;
using UniRx.Async;

public sealed class ManualDialog : BaseDialog
{
    public void Open() {
        Show().Forget();
    }

    public void OnClickCloseButton() {
        Hide().Forget();
        GameManager.Audio.PlayButtonSFX();
    }
}
