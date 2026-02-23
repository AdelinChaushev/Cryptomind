import Hero from '../components/Hero';
import FeaturedCiphers from '../components/FeaturedCiphers';
import HowItWorks from '../components/HowItWorks';
import Features from '../components/Features';
import CTA from '../components/CTA';


const Home = () => {
    return (
        <>
            {/* <Navbar /> */}
            <Hero />
            <FeaturedCiphers />
            <HowItWorks />
            <Features />
            <CTA />
        </>
    );
};

export default Home;
