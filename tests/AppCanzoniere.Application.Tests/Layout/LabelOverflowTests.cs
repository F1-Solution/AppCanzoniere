using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

public class LabelOverflowTests
{
    [Fact]
    public void AWrappedSectionLabelWiderThanTheWidth_CarriesTheOverflowFlag()
    {
        Assert.True(OnlySection(Flow("{c: Ab}\nUno", Options(5))).LabelOverflow);
        Assert.False(OnlySection(Flow("{c: Ab}\nUno", Options(400))).LabelOverflow);
        Assert.False(OnlySection(Flow("Uno", Options(400))).LabelOverflow);
    }
}
