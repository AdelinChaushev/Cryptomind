import HeroTypewriter from './HeroTypewriter';

const Hero = () => {
    return (
        <section className="hero">
            <HeroTypewriter />
            <p>
                Предизвикайте себе си с класически шифри, учете се от AI-подсказки
                и се състезавайте с други криптографи. 14 вида шифри,
                разкрийте тайните на криптирането.
            </p>
            <div className="hero-buttons">
                <a href="/register" className="btn btn-primary btn-hero">
                    Започни да решаваш
                </a>
            </div>
        </section>
    );
};

export default Hero;