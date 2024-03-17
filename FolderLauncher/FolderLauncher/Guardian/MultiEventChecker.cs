
namespace FolderLauncher.Guardian
{
    /// <summary>
    /// イベントから同一イベントが単位時間内に複数回発生したかを判定するクラス
    /// </summary>
    /// <remarks>
    /// 例：MouseUpイベントが2重に発生したかを判定することで、ダブルクリックが成立したかを判定する
    /// 例：右マウスUpイベントが2重に発生したかを判定することで、右クリックが成立したかを判定する
    /// </remarks>
    internal class MultiEventChecker
    {
        /// <summary>
        /// マルチイベント発生と判定する時間幅(ミリ秒)
        /// </summary>
        private int eventTimeSpan;

        /// <summary>
        /// 最後に単一イベントが発生した時間
        /// </summary>
        private int firstEventTime;

        /// <summary>
        /// 対象のイベントを表すwParam
        /// </summary>
        private IntPtr targetWParam;

        /// <summary>
        /// マルチイベント成立のための単一イベントの発生回数
        /// </summary>
        private int eventCountForMultiEvent;

        /// <summary>
        /// 最初に単一イベントが発生してから、単位時間までに発生した単一イベントの回数
        /// </summary>
        private int currentCount;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="targetWParam">対象のイベントを表すwParam</param>
        /// <param name="eventTimeSpan">マルチイベント発生と判定する時間幅(ミリ秒)</param>
        /// <param name="eventCount">マルチイベント成立のための単一イベントの発生回数</param>
        public MultiEventChecker(IntPtr targetWParam,int eventTimeSpan,int eventCount)
        {
            // 妥当性確認
            if(eventCount < 2)
            {
                throw new ArgumentException("eventCount must be 2 or more");
            }

            this.eventCountForMultiEvent = eventCount;
            this.targetWParam = targetWParam;
            this.eventTimeSpan = eventTimeSpan;
            this.currentCount = 0;
            firstEventTime = 0;
        }

        /// <summary>
        /// 単一イベント時にマルチイベントが成立したか否かを判定する
        /// </summary>
        /// <param name="wParam">イベントパラメータ</param>
        /// <returns>マルチイベントが成立したか</returns>
        public bool IsMultiEventInvoked(IntPtr wParam)
        {
            // 指定のイベント以外は無視する
            if(wParam != targetWParam)
            {
                return false;
            }

            var currentTime = Environment.TickCount;

            // 初回イベント時を記録する 
            if(currentCount == 0)
            {
                firstEventTime = currentTime;
            }

            //経過時間を取得
            var elapsedTime = currentTime - firstEventTime;

            // 経過時間判定
            if(elapsedTime > eventTimeSpan)
            {
                // 初回として扱う
                firstEventTime = currentTime;
                currentCount = 1;
                return false;
            }

            // 単一イベント発生回数判定
            if(++currentCount == eventCountForMultiEvent)
            {
                currentCount = 0;
                return true;
            }

            return false;
        }
    }
}
