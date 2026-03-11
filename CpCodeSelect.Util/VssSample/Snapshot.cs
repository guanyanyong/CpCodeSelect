using Alphaleonis.Win32.Vss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpCodeSelect.Util.VssSample
{
    public class Snapshot : IDisposable
    {
        private IVssBackupComponents _backup;
        private Guid _snapshotSetId;
        private Guid _snapshotId;

        public Snapshot(IVssBackupComponents backup)
        {
            _backup = backup;
            _snapshotSetId = _backup.StartSnapshotSet();
        }

        public void AddVolume(string volumeName)
        {
            _snapshotId = _backup.AddToSnapshotSet(volumeName, Guid.Empty);
        }

        public void Dispose()
        {
            // 清理快照
            if (_backup != null && _snapshotSetId != Guid.Empty)
            {
                try { _backup.DeleteSnapshotSet(_snapshotSetId, false); } catch { }
            }
        }
    }
}