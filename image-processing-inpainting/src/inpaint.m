function [Result, ResultSmooth] =       ...
             inpaint(Filename,          ... % Input file name:          ex. 'test_images/houses_text.png'
                     B,                 ... % Block size:               ex. 16
                     SearchWindowSize,  ... % Search window size:       ex. 31
                     brute_force,       ... % Brute force:              ex. false
                     slicing,           ... % Slicing/iterations:       ex. true
                     slicing_erosion,   ... % Erosion for iterations:   ex. true
                     mask_erosion_size, ... % Pixels to erode:          ex. 3
                     noise_level,       ... % Max noise:                ex. 20
                     smooth_bilateral,  ... % Smoothing at end:         ex. true
                     smooth_msd,        ... % Smoothing for MSD:        ex. false
                     copy_from_smoothed,... % Inpaint w/ smooth pixel:  ex. false
                     smooth_adaptive,   ... % Smooth based on iteration ex. true
                     smooth_unknown,    ... % Smooth based on #unknown  ex. true
                     scratch_detection, ... % Use scratch detection:    ex. true
                     show_results)          % Show images:              ex. true
                 
    %% Additional parameters
    threshold = 50 * 3;                       % Threshold of color difference to erode mask
    % W = ceil(B/2);                          % Half the size of one block
    padding = floor(B/2);                     % Image is padded with half the block size at each side, so we can inpaint pixels next to border of image
    W_before = floor(B/2);                    % Amount of pixels left/above the center pixel in a block
    W_after = floor(B/2);                     % Amount of pixels right/under the center pixel in a block
    if(mod(B,2) == 0)
        W_after = W_after - 1;                % If block size is even, center pixel is first pixel of right/lower half of block
    end
    Image = double(imread(Filename));
    [M,N,C] = size(Image);

    % If slicing == true and slicing_erosion == false, we only inpaint the pixels with the most pixels known around them (in iterations)
    % Only then is the following parameter used to calculate amounts for slicing by division through this parameter
    slicing_factor = B*3;

    if smooth_unknown
        % if smooth_unknown == true, the area to smooth around a pixel is calculated by dividing through this smoothing factor (lower=more smoothing).
        smoothing_factor = (B^2)/5;
    else
        % if only smooth_adaptive == true, the area to smooth around a pixel is based on the number of the iteration when the pixel was inpainted.
        smoothing_factor = 0.75;
    end

    %% TASK 2.2
    % Detect pixels that need to be inpainted (Mask=0: inpaint, Mask=1: leave untouched)
    if (scratch_detection)
        Mask = xor(detect_scratch(Filename), 1);
    else
        Mask = xor(detect_marking(Image, 40, 8), 1); % detect_marking's output is the inverse, so we need to xor it
    end

    %% Add noise to input image
    ImageBeforeNoise = Image;
    if (noise_level > 0)
        Image = add_noise(Image, noise_level);
        % Smoothing should be done after image is inpainted
    end

    %% Erode empty lines in the mask, results in noticable improvements for
     % the even areas but artifacts in textured areas
    [x, y] = size(Mask);
    for i = 2:x
        width = 0;
        skip = 0;
        for j = 1:y
            if skip > 0
                skip = skip - 1;
                continue
            end
            while Mask(i,j + width) == 0
                width = width + 1;
                if j + width >= y
                    break
                end
            end
            if width > 2
                pad = floor(B/2);
                start = j - pad;
                if j <= pad
                    start = 1;
                end
                stop = j + width + pad;
                if j + width + pad >= y
                    stop = y;
                end
                startColor = Image(i, start, 1) + Image(i, start, 2) + Image(i, start, 3);
                stopColor = Image(i, stop, 1) + Image(i, stop, 2) + Image(i, stop, 3);
                if (abs(startColor - stopColor) < threshold)
                    Mask(i,start:stop) = imerode(Mask(i,start:stop), strel('line', floor((stop-start)/pad), 0));
                end
                if j + width >= y
                    break
                end
            end
            skip = width;
            width = 0;
        end
    end

    %% Prepare image for inpainting
    % Set the pixels that need to be inpainted to zero
    InpaintedImage = Image .* Mask(:,:,ones(1,1,3));
    % Add padding
    InpaintedImage = padarray(InpaintedImage, [padding padding 0], 'symmetric');
    Mask = padarray(Mask, [padding padding], 'symmetric');
    total = sum(sum(Mask(padding+1:M+padding, padding+1:N+padding)==0)); % total number of pixels to inpaint
    left = total; % number of pixels left to inpaint
    iter = 0; % iteration number
    StartMask = Mask; % Save mask that we started with for later use
    AmountOfSmooth = zeros(size(StartMask));
    
    %% Iterate
    while left>0 && iter<100
        iter = iter + 1;
        InpaintedImage_New = InpaintedImage;
        InpaintedMask = zeros(size(Mask));
        %% Slicing
        if slicing
            if ~slicing_erosion
                %% Slicing by selecting the pixels with the most known pixels in the block around them
                Slice = zeros(size(Mask));
                IntegralMask = intimage(Mask);
                for i = padding+1:M+padding
                    for j =  padding+1:N+padding
                        if Mask(i,j) == 0
                            Slice(i,j) = floor((IntegralMask(i+W_after+1,j+W_after+1)-IntegralMask(i+W_after+1,j-W_before)-IntegralMask(i-W_before,j+W_after+1)+IntegralMask(i-W_before,j-W_before))/slicing_factor);
                        end
                    end
                end
                maxKnownPixels = max(max(Slice));
                Slice(Slice < maxKnownPixels) = 1; %Inpainted pixels and pixels with not much known pixels in block are set to 1
                Slice(Slice == maxKnownPixels) = 0;
                Slice = or(Slice, Mask); % In case maxKnownPixels is 1, we don't want everything to be set to 0.
            else
                %% Slicing with erosion
                Slice = ~Mask;
                se = strel('disk',mask_erosion_size);
                Slice = imerode(Slice,se);
                Slice = or(Slice, Mask);
            end
        else
            Slice = Mask;
        end
        AmountOfSmooth = AmountOfSmooth + ((~Slice) * iter);
        
        %% Prepare for inpainting algorithm
        if smooth_msd
            OriginalImage = double(smooth_image(InpaintedImage, 3, Mask));
        else
            OriginalImage = InpaintedImage;
        end
        msd = Inf(M+padding*2,N+padding*2);

        % Incorrect for even search window sizes, like 30. -15:15 is 31 pixels.
        % Solution: If search window size is even, center pixel is first pixel of right/lower half of search window
        range_Delta_m = -floor(SearchWindowSize/2):floor(SearchWindowSize/2);
        range_Delta_n = -floor(SearchWindowSize/2):floor(SearchWindowSize/2);
        if(mod(B,2) == 0)
            range_Delta_m = -floor(SearchWindowSize/2):(floor(SearchWindowSize/2)-1);
            range_Delta_n = -floor(SearchWindowSize/2):(floor(SearchWindowSize/2)-1);
        end
        
        %% Iterate over search window
        for Delta_m = range_Delta_m,
            for Delta_n = range_Delta_n,       
                % non-zero displacement
                if (Delta_m~=0 || Delta_n~=0),
                    fprintf('.');

                    % compute the feasible ranges for m and n (to stay within the image boundaries)
                    % Provided code assumed incorrect block width and couldn't fill in pixels at the edges either
                    m_min = max(min(1-Delta_m, M), 1);
                    m_max = min(max(M-Delta_m, 1), M);
                    n_min = max(min(1-Delta_n, N), 1);
                    n_max = min(max(N-Delta_n, 1), N);
                    range_m = padding+(m_min:m_max);
                    range_n = padding+(n_min:n_max);

                    if n_min>n_max || m_min>m_max,
                        continue;
                    end;

                    DisplacedMask = circshift(Mask, [-Delta_m -Delta_n]);
                    UsedPixelsMask = Mask .* DisplacedMask;

                    ReferenceImage = circshift(OriginalImage, [-Delta_m -Delta_n]);

                    if(~brute_force)
                        IntegralMask = intimage(UsedPixelsMask);
                        IntegralImage = sum(intimage((OriginalImage - ReferenceImage).^2 .* repmat(UsedPixelsMask,[1 1 3])),3);

                        % MSE based on perceived brightness - failed attempt
                        % IntegralImage = intimage((InpaintedImage - DisplacedImage).^2 .* repmat(UsedPixelsMask,[1 1 3]));
                        % IntegralImage = 0.299 .* IntegralImage(:,:,1) + 0.587 .* IntegralImage(:,:,2) + 0.114 .* IntegralImage(:,:,3);

                        % We now add a row and a column of zeros to IntegralImage because when the region
                        % we want to compute is adjacent to the side of the picture, we need a value of zero
                        % for the blocks off-screen, e.g. s(x, ?1) = ii(?1, y) = 0.
                        IntegralMask = padarray(IntegralMask, [1 1], 0, 'pre');
                        IntegralImage = padarray(IntegralImage, [1 1], 0, 'pre');
                    else
                        SDImage = (OriginalImage - ReferenceImage).^2 .* repmat(UsedPixelsMask,[1 1 3]);
                    end
                    
                    %% Iterate over pixels in image that can be inpainted with this displacement
                    for i = range_m
                        for j = range_n
                            if (Slice(i,j) == 0 && Mask(i+Delta_m, j+Delta_n) == 1)
                                % 1 is added to every index because we added a row and a column of zeros to IntegralImage and IntegralMask.
                                if(~brute_force)
                                    image_block = (IntegralImage(i+W_after+1,j+W_after+1)-IntegralImage(i+W_after+1,j-W_before)-IntegralImage(i-W_before,j+W_after+1)+IntegralImage(i-W_before,j-W_before));
                                    mask_block = (IntegralMask(i+W_after+1,j+W_after+1)-IntegralMask(i+W_after+1,j-W_before)-IntegralMask(i-W_before,j+W_after+1)+IntegralMask(i-W_before,j-W_before));
                                else
                                    image_block = sum(sum(sum(SDImage(i-W_before:i+W_after,j-W_before:j+W_after,:))));
                                    mask_block = sum(sum(UsedPixelsMask(i-W_before:i+W_after,j-W_before:j+W_after)));
                                end
                                % mask_block = B^2;
                                current_msd = image_block/mask_block;
                                if (msd(i,j) > current_msd)
                                    if copy_from_smoothed
                                        InpaintedImage_New(i,j,:) = OriginalImage(i+Delta_m, j+Delta_n,:);
                                    else
                                        InpaintedImage_New(i,j,:) = InpaintedImage(i+Delta_m, j+Delta_n,:);
                                    end
                                    msd(i,j) = current_msd;
                                    InpaintedMask(i,j) = 1;
                                end
                            end
                        end
                    end
                end;
            end;
        end;

        % Update the mask according to the pixels that were inpainted in the last
        % iteration.
        Mask = or(InpaintedMask, Mask);

        % compute the number of pixels left
        left=sum(sum(Mask(padding+1:M+padding, padding+1:N+padding)==0));
        if show_results
            figure;
            subplot(1,2,1),imagesc(uint8(Image)),title('Degraded image (no padding)');
            subplot(1,2,2),imagesc(uint8(InpaintedImage_New)),title('Inpainted image (with padding)');
            pause(0.1);
        end
        fprintf('\n');

        % go to the next iteration
        InpaintedImage = padarray(InpaintedImage_New(padding+1:M+padding, padding+1:N+padding, :), [padding padding 0], 'symmetric');
        Mask = padarray(Mask(padding+1:M+padding, padding+1:N+padding), [padding padding], 'symmetric');
    end;
    
    %% Smooth inpainted image based on number of unknown pixels around it
    if smooth_adaptive
        Result = InpaintedImage;
        if smooth_unknown
            AmountOfSmooth = zeros(size(StartMask));
            IntegralMask = intimage(StartMask);
            for i = padding+1:M+padding
                for j =  padding+1:N+padding
                    if StartMask(i,j) == 0
                        AmountOfSmooth(i,j) = (IntegralMask(i+W_after+1,j+W_after+1)-IntegralMask(i+W_after+1,j-W_before)-IntegralMask(i-W_before,j+W_after+1)+IntegralMask(i-W_before,j-W_before));
                    end
                end
            end
            fprintf('Smoothing based on number of unknown pixels. This number is: ');
        else
            AmountOfSmooth = padarray(AmountOfSmooth(padding+1:M+padding, padding+1:N+padding), [padding padding], 0);
            fprintf('Adaptive smoothing. Level of smoothing is: ');
        end
        maxKnownPixels = max(max(AmountOfSmooth));
        dividedKnownPixels = round(maxKnownPixels/smoothing_factor);
        SmoothedImage = InpaintedImage;
        while dividedKnownPixels > 0
            fprintf('%d (Smoothing factor: %d), ', maxKnownPixels, dividedKnownPixels);
            SmoothedImage(:,:,1) = filter2(fspecial('average',dividedKnownPixels),InpaintedImage(:,:,1));
            SmoothedImage(:,:,2) = filter2(fspecial('average',dividedKnownPixels),InpaintedImage(:,:,2));
            SmoothedImage(:,:,3) = filter2(fspecial('average',dividedKnownPixels),InpaintedImage(:,:,3));
            for i = padding+1:M+padding
                for j = padding+1:N+padding
                    if round(AmountOfSmooth(i,j)/smoothing_factor) == dividedKnownPixels
                        Result(i,j,:)=SmoothedImage(i,j,:);
                        AmountOfSmooth(i,j) = 0;
                    end
                end
            end
            % AmountOfSmooth(AmountOfSmooth == maxKnownPixels) = 0;
            maxKnownPixels = max(max(AmountOfSmooth));
            dividedKnownPixels = round(maxKnownPixels/smoothing_factor);
        end
        fprintf('0.\n');
        InpaintedImage = Result;
    end

    %% Finalize inpainted image
    % Remove padding
    InpaintedImage = InpaintedImage(padding+1:M+padding, padding+1:N+padding, :);
    Mask = Mask(padding+1:M+padding, padding+1:N+padding);
    Result = InpaintedImage;
    ResultSmooth = Result;
    % Display results
    if show_results
        figure;
        subplot(1,2,1),imagesc(uint8(Image)),title('Degraded image (no padding)');
        subplot(1,2,2),imagesc(uint8(InpaintedImage)),title('Inpainted image (no padding)');
    end
    %% Post-inpainting noise reduction w/ bilateral filter
    if smooth_bilateral
        w = 5;  % bilateral filter half-width
        sigma = [3 0.14]; % bilateral filter standard deviations

        % Apply bilateral filter to each image.
        SmoothedImage = double(InpaintedImage)/255; % Rescale for bfilter2
        SmoothedImage = bfilter2(SmoothedImage,w,sigma);
        SmoothedImage = SmoothedImage * 255; % Rescale for plot
        ResultSmooth = SmoothedImage;

        if show_results
            figure;
            subplot(2,2,1),imagesc(uint8(ImageBeforeNoise)),title('Original image (before noise)');
            subplot(2,2,2),imagesc(uint8(Image)),title('Original image (noisy)');
            subplot(2,2,3),imagesc(uint8(InpaintedImage)),title('Inpainted image (noisy)');
            subplot(2,2,4),imagesc(uint8(SmoothedImage)),title('Inpainted image (smoothed)');
        end
    end
end


