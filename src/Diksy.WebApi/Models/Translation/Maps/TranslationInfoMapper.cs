using TranslationInfoModel = Diksy.Translation.Models.TranslationInfo;
using TranslationInfoDto = Diksy.WebApi.Models.Translation.TranslationInfo;

namespace Diksy.WebApi.Models.Translation.Maps
{
    public static class TranslationInfoMapper
    {
        public static TranslationInfoDto MapFrom(TranslationInfoModel translationInfo)
        {
            ArgumentNullException.ThrowIfNull(translationInfo);

            return new TranslationInfoDto
            {
                Phrase = translationInfo.Phrase,
                Translation = translationInfo.Translation,
                Transcription = translationInfo.Transcription,
                Example = translationInfo.Example,
                TranslationOfExample = translationInfo.TranslationOfExample
            };
        }

        public static TranslationInfoModel MapTo(TranslationInfoDto translationInfoDto)
        {
            ArgumentNullException.ThrowIfNull(translationInfoDto);

            return new TranslationInfoModel
            {
                Phrase = translationInfoDto.Phrase,
                Translation = translationInfoDto.Translation,
                Transcription = translationInfoDto.Transcription,
                Example = translationInfoDto.Example,
                TranslationOfExample = translationInfoDto.TranslationOfExample
            };
        }
    }
}