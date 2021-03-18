using Microsoft.Xna.Framework.Audio;

namespace RocketUI.Audio.Builtin
{
	public class MonogameSoundEffect : ISoundEffect
	{
		private SoundEffect _soundEffect;
		public MonogameSoundEffect(SoundEffect soundEffect)
		{
			_soundEffect = soundEffect;
		}
		
		/// <inheritdoc />
		public void Play()
		{
			_soundEffect.Play();
		}
		
		public static implicit operator MonogameSoundEffect(SoundEffect effect)
		{
			return new MonogameSoundEffect(effect);
		}
	}
}