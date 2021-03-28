void nanCheck( inout Particle p ){

    bool nan;

    nan = isnan(p.vel);
    if( nan ){ p.vel = 0; }

    nan = isnan(p.pos);
    if( nan ){ p.pos = 0; }

    nan = isnan(p.nor);
    if( nan ){ p.nor = float3(1,0,0); }

}