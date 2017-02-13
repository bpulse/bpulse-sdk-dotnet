using me.bpulse.domain.proto.collector;

namespace bpulse_sdk_csharp.pulseRepository
{
    public interface IRepository
    {
        #region Public Methods

        int CountBpulsesRq();

        int CountMarkBpulseKeyInProgress();

        void DeleteBpulseRqByKey(string pKey);

        PulsesRQ GetBpulseRqByKey(string pKey);

        long GetDbSize();

        object[] GetSortedbpulseRqMapKeys();

        void MarkBpulseKeyInProgress(string pKey);

        void ReleaseBpulseKeyInProgressByKey(string pKey);

        void SavePulse(PulsesRQ pPulsesRQ);

        #endregion Public Methods
    }
}