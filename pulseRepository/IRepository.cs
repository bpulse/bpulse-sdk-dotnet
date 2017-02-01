using me.bpulse.domain.proto.collector;
using System;

namespace bpulse_sdk_csharp.pulseRepository
{
    public interface IRepository
    {
        void SavePulse(PulsesRQ pPulsesRQ);

        object[] getSortedbpulseRQMapKeys();

        int countBpulsesRQ();

        int countMarkBpulseKeyInProgress();

        PulsesRQ getBpulseRQByKey(string pKey);

        void deleteBpulseRQByKey(string pKey);

        void markBpulseKeyInProgress(string pKey);

        void releaseBpulseKeyInProgressByKey(string pKey);

        long getDBSize();
    }
}