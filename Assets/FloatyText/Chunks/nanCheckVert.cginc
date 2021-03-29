void nanCheck( inout Vert v ){

    bool nan;

    nan = isnan(v.vel);
    if( nan ){ v.vel = 0; }

    nan = isnan(v.pos);
    if( nan ){ v.pos = 0; }

    nan = isnan(v.nor);
    if( nan ){ v.nor = float3(1,0,0); }

}