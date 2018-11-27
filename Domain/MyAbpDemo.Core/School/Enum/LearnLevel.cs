using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MyAbpDemo.Core
{
    public enum LearnLevel:byte
    {
        [DisplayName("不合格")]
        BelowStandard=1,

        [DisplayName("合格")]
        Standard = 2,

        [DisplayName("良好")]
        Well = 3,

        [DisplayName("优秀")]
        Excellent = 4
    }
}
