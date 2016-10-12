% It's probably a good idea to set the parameters before you run the script, since the function definition is so long.

%% Fence_mask: adaptive smoothing (no noise)
Filename = 'test_images/fence_mask.png';
B = 4;
SearchWindowSize = 8;
brute_force = false;
slicing = true;
slicing_erosion = true;
mask_erosion_size = 3;
noise_level = 0;
smooth_bilateral = false;
smooth_msd = false;
copy_from_smoothed = false;
smooth_adaptive = true;
smooth_unknown = false;
detect_scratches = false;
show_results = false;
[fence_inpainted, ~] = inpaint(Filename,B,SearchWindowSize,brute_force, ...
    slicing,slicing_erosion,mask_erosion_size,noise_level,smooth_bilateral,smooth_msd, ...
    copy_from_smoothed,smooth_adaptive,smooth_unknown,detect_scratches,show_results);
imwrite(uint8(fence_inpainted),'results/fence_inpainted.png');

%% Houses_text: with noise 
Filename = 'test_images/houses_text.png';
B = 16;
SearchWindowSize = 31;
brute_force = false;
slicing = true;
slicing_erosion = true;
mask_erosion_size = 3;
noise_level = 20;
smooth_bilateral = true;
smooth_msd = false;
copy_from_smoothed = false;
smooth_adaptive = false;
smooth_unknown = false;
detect_scratches = false;
show_results = true;
[houses_inpainted, houses_inpainted_smooth] = inpaint(Filename,B,SearchWindowSize,brute_force, ...
    slicing,slicing_erosion,mask_erosion_size,noise_level,smooth_bilateral,smooth_msd, ...
    copy_from_smoothed,smooth_adaptive,smooth_unknown,detect_scratches,show_results);
imwrite(uint8(houses_inpainted),'results/houses_noise_inpainted.png');
imwrite(uint8(houses_inpainted_smooth),'results/houses_noise_smoothed.png');

%% Houses_text: no noise
B = 16;
SearchWindowSize = 31;
noise_level = 0;
smooth_bilateral = false;
[houses_inpainted, ~] = inpaint(Filename,B,SearchWindowSize,brute_force, ...
    slicing,slicing_erosion,mask_erosion_size,noise_level,smooth_bilateral,smooth_msd, ...
    copy_from_smoothed,smooth_adaptive,smooth_unknown,detect_scratches,show_results);
imwrite(uint8(houses_inpainted),'results/houses_clean_16_inpainted.png');

%% Houses_text: no noise
% B = 8;
% SearchWindowSize = 31;
% noise_level = 0;
% smoothing = false;
% [houses_inpainted, ~] = inpaint(Filename,B,SearchWindowSize,brute_force, ...
%     slicing,slicing_erosion,mask_erosion_size,noise_level,smoothing, ...
%     smooth_msd,copy_from_smoothed,detect_scratches,show_results);
% imwrite(uint8(houses_inpainted),'results/houses_clean_8_inpainted.png');

%% Scratch detection
Filename = 'test_images/ww1_original.png';
B = 8;
SearchWindowSize = 31;
noise_level = 0;
smooth_bilateral = false;
detect_scratches = true;
[ww1_inpainted, ~] = inpaint(Filename,B,SearchWindowSize,brute_force, ...
    slicing,slicing_erosion,mask_erosion_size,noise_level,smooth_bilateral,smooth_msd, ...
    copy_from_smoothed,smooth_adaptive,smooth_unknown,detect_scratches,show_results);
imwrite(uint8(ww1_inpainted),'results/ww1_inpainted.png');
