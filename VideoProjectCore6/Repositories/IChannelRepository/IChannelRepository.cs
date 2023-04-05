using VideoProjectCore6.DTOs.ChannelDto;

namespace VideoProjectCore6.Repositories.IChannelRepository
{
    public interface IChannelRepository
    {
        List<ChannelGetDto> GetChannelsName(string lang);
        ChannelMailFirstSetting GetChannelMailFirstConfig();
        void AddChannelMailFirstSetting(ChannelMailFirstSetting channelMailFirstSetting);
        void TestEmailFirstSettingConnection(ChannelMailFirstSetting config, string lang);
    }
}
