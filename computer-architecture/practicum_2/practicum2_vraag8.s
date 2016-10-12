lucas:
	movb	%al, %cl
	movl	$2, %eax
	shl	%cl, %eax
	movl	$3, %ebx
	subl	$1, %eax
	testb	$1, %cl
	jz	.L3
.L2:
	movb	$1, %cl
	shl	%eax
	divl	%ebx
	ret
.L3:
	addl	$2, %eax
	jmp	.L2